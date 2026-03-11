pipeline {
    agent any

    environment {
        AWS_REGION     = 'ap-south-1'
        AWS_ACCOUNT_ID = '533267129063'
        ECR_REPO       = 'employee-webapp'
        ECS_CLUSTER    = 'employee-webapp-cluster'
        ECS_SERVICE    = 'employee-webapp-service'

        ECR_REGISTRY   = "$533267129063.dkr.ecr.ap-south-1.amazonaws.com/employee-webapp"
        IMAGE_TAG      = "${env.BUILD_NUMBER}-${env.GIT_COMMIT.take(7)}"
        FULL_IMAGE     = "${ECR_REGISTRY}/${ECR_REPO}:${IMAGE_TAG}"
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
                echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
                echo "Branch    : ${env.GIT_BRANCH}"
                echo "Commit    : ${env.GIT_COMMIT}"
                echo "Image tag : ${IMAGE_TAG}"
                echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
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
            echo """
            ══════════════════════════════════════════
            PIPELINE SUCCESS — TBD + IAM Role
            Image   : ${FULL_IMAGE}
            Branch  : ${env.GIT_BRANCH}
            Cluster : ${ECS_CLUSTER}
            ervice : ${ECS_SERVICE}
            ══════════════════════════════════════════
            """
        }
        failure {
            echo "PIPELINE FAILED — scroll up to find which stage failed"
        }
        always {
            sh "docker rmi ${FULL_IMAGE} || true"
            sh "docker rmi ${ECR_REGISTRY}/${ECR_REPO}:latest || true"
        }
    }
}
