pipeline {
    agent any

    environment {
        AWS_REGION     = 'ap-south-1'
        AWS_ACCOUNT_ID = '533267129063'
        ECR_REPO       = 'employee-webapp'
        ECS_CLUSTER    = 'employee-webapp-cluster'
        ECS_SERVICE    = 'employee-webapp-service'

        ECR_REGISTRY   = "${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com"
        IMAGE_TAG      = "${env.BUILD_NUMBER}-${env.GIT_COMMIT.take(7)}"
        FULL_IMAGE     = "${ECR_REGISTRY}/${ECR_REPO}:${IMAGE_TAG}"
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
                echo "Branch    : ${env.GIT_BRANCH}"
                echo "Commit    : ${env.GIT_COMMIT}"
                echo "Image tag : ${IMAGE_TAG}"
            }
        }

        stage('Build Docker Image') {
            steps {
                sh """
                    docker build -t ${FULL_IMAGE} .
                    docker tag ${FULL_IMAGE} ${ECR_REGISTRY}/${ECR_REPO}:latest
                    echo "Image built: ${FULL_IMAGE}"
                """
            }
        }

        stage('Trivy Security Scan') {
            steps {
                sh """
                    if ! command -v trivy &> /dev/null; then
                        echo "Installing Trivy..."
                        sudo apt-get install -y wget apt-transport-https gnupg
                        wget -qO - https://aquasecurity.github.io/trivy-repo/deb/public.key | \
                            sudo gpg --dearmor -o /usr/share/keyrings/trivy.gpg
                        echo "deb [signed-by=/usr/share/keyrings/trivy.gpg] https://aquasecurity.github.io/trivy-repo/deb generic main" | \
                            sudo tee /etc/apt/sources.list.d/trivy.list
                        sudo apt-get update
                        sudo apt-get install -y trivy
                    fi

                    trivy image \
                        --severity CRITICAL,HIGH \
                        --no-progress \
                        --format table \
                        ${FULL_IMAGE}

                    trivy image \
                        --severity CRITICAL,HIGH,MEDIUM \
                        --no-progress \
                        --format json \
                        --output trivy-report.json \
                        ${FULL_IMAGE}

                    echo "Trivy scan complete"
                """
            }
        }

        stage('Push to ECR') {
            steps {
                sh """
                    aws ecr get-login-password --region ${AWS_REGION} | \
                    docker login \
                        --username AWS \
                        --password-stdin ${ECR_REGISTRY}

                    docker push ${FULL_IMAGE}
                    docker push ${ECR_REGISTRY}/${ECR_REPO}:latest

                    echo "Pushed to ECR: ${FULL_IMAGE}"
                """
            }
        }

        stage('Deploy to ECS') {
            steps {
                sh """
                    aws ecs update-service \
                        --cluster         ${ECS_CLUSTER} \
                        --service         ${ECS_SERVICE} \
                        --force-new-deployment \
                        --region          ${AWS_REGION}

                    echo "ECS deployment triggered"
                    echo "Image: ${FULL_IMAGE}"
                """
            }
        }
    }

    post {
        success {
            echo "PIPELINE SUCCESS - Image: ${FULL_IMAGE}"
        }
        failure {
            echo "PIPELINE FAILED - scroll up to find which stage failed"
        }
        always {
            archiveArtifacts artifacts: 'trivy-report.json', allowEmptyArchive: true
            sh "docker rmi ${FULL_IMAGE} || true"
            sh "docker rmi ${ECR_REGISTRY}/${ECR_REPO}:latest || true"
        }
    }
}
