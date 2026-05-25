# Employee Webapp — CI/CD with Trunk Based Development

A .NET 8 web application deployed to AWS ECS Fargate using two CI/CD pipelines
(Jenkins and GitHub Actions) following Trunk Based Development (TBD) strategy.

---

## 🏗️ Architecture
Developer
│
├── feature branch (1-2 days max)
│         │
│      Pull Request
│         │
▼         ▼
main (trunk)
│
┌─────┴─────────────┐
│                   │
▼                   ▼
Jenkins              GitHub Actions
(IAM Role)           (OIDC)
│                   │
└─────────┬─────────┘
│
┌─────▼──────┐
│  Pipeline   │
│  5 Stages   │
└─────┬──────┘
│
┌──────────┼──────────┐
▼          ▼          ▼
Build       Trivy      Push
Image       Scan       ECR
│
▼
Deploy ECS
Fargate
│
▼
App Live 🚀

---

## 🛠️ Tech Stack

| Category | Tool |
|---|---|
| Application | .NET 8 (ASP.NET Core) |
| Containerization | Docker (multi-stage build) |
| Container Registry | AWS ECR |
| Deployment | AWS ECS Fargate |
| CI/CD Tool 1 | Jenkins |
| CI/CD Tool 2 | GitHub Actions |
| Security Scan | Trivy |
| Auth — Jenkins | AWS IAM Role (EC2 Instance Profile) |
| Auth — GitHub Actions | OIDC (JWT → AWS STS) |
| Strategy | Trunk Based Development (TBD) |
| Region | AWS ap-south-1 (Mumbai) |

---

## 🌿 TBD Strategy
❌ Gitflow : dev → stage → prod (3 branches, 3 pipelines)
✅ TBD     : main only (1 branch, 1 pipeline)
Rules:
→ Only main is long-lived
→ Feature branches live max 1-2 days
→ Every PR merge to main triggers pipeline
→ Environments = pipeline stages, not branches
→ main is always in deployable state

---

## 🔐 Authentication

### Jenkins — IAM Role
EC2 Instance → IAM Role attached → AWS CLI inherits
temp credentials automatically from instance metadata.
Zero stored credentials in Jenkins.

### GitHub Actions — OIDC
GitHub Runner → JWT token → AWS STS verifies
→ issues temporary credentials → pipeline runs.
No credentials stored anywhere.

---

## ⚙️ Pipeline Stages
Stage 1 → Checkout       pull code from main branch
Stage 2 → Build Image    docker build, tag = buildNo-commitSHA
Stage 3 → Trivy Scan     scan CRITICAL and HIGH CVEs
Stage 4 → Push ECR       push image to private registry
Stage 5 → Deploy ECS     rolling update on Fargate

---

## 🐳 Dockerfile

Multi-stage build to keep final image small:
Stage 1 (Build)   : .NET SDK 8.0   ~800MB
compiles app
publishes artifacts
Stage 2 (Runtime) : ASP.NET 8.0    ~200MB
copies published files only
runs the app

---

## 📦 Image Tagging Strategy
Format  : buildNumber-commitSHA
Example : 9-eec8108
→ buildNumber : which pipeline run
→ commitSHA   : exact git commit
→ Combined    : full traceability from
running container → source code

---

## ☁️ AWS Infrastructure

| Resource | Name |
|---|---|
| ECR Repository | employee-webapp |
| ECS Cluster | employee-webapp-cluster |
| ECS Service | employee-wedapp-service |
| Task Definition | employee-wedapp-task |
| IAM Role (Jenkins) | Jenkins-ECR-ECS-Role |
| IAM Role (ECS Task) | ecsTaskExecutionRole |
| IAM Role (GitHub) | GitHubActions-ECR-ECS-Role |

---

## 🔍 Security Scan Results
Tool    : Trivy v0.69.3
Image   : employee-webapp (Debian 12.13)
Total   : 3 vulnerabilities (HIGH: 2, CRITICAL: 1)
All vulnerabilities are in the base Debian image.
No vulnerabilities found in application code.
Status  : Accepted risk (no fix available upstream)

---

## 📁 Project Structure
employee-devops-dotnet/
├── employee-webapp.csproj
├── Program.cs
├── Models/
│   └── Employee.cs
├── Controllers/
├── Views/
├── Dockerfile                        ← multi-stage build
├── Jenkinsfile                       ← Day 1 pipeline
└── .github/
└── workflows/
└── deploy.yml                ← Day 2 pipeline

---

## 🚀 How to Run Locally

```bash
# Clone repo
git clone https://github.com/Anil7749/employee-devops-dotnet.git
cd employee-devops-dotnet

# Build docker image
docker build -t employee-webapp .

# Run locally
docker run -p 9090:8080 employee-webapp

# Open browser
http://localhost:9090
```

---

## 🔄 How to Trigger Pipeline

```bash
# Create feature branch
git checkout -b feature/your-feature

# Make changes
git add .
git commit -m "feat: your change"
git push origin feature/your-feature

# Raise PR on GitHub
# base: main ← compare: feature/your-feature
# Merge PR → pipeline triggers automatically
```

---

## 📊 Jenkins vs GitHub Actions

| | Jenkins | GitHub Actions |
|---|---|---|
| Server | Self-hosted EC2 | GitHub managed |
| Syntax | Groovy | YAML |
| Auth | IAM Role | OIDC |
| Trigger | GitHub Webhook | Native git events |
| Cost | EC2 running cost | Free (public repos) |
| Setup | Manual | Minimal |
| Best for | Enterprise | Cloud native |

---

## 📝 Key Learnings

TBD eliminates long-lived branches and merge conflicts
IAM Roles remove need for static AWS credentials
OIDC is more secure than IAM Roles — no server needed
Multi-stage Docker builds reduce image size significantly
Trivy catches vulnerabilities before they reach production
Image tagging with SHA enables full deployment traceability


---

## 👤 Author

**Anil**
GitHub: [@Anil7749](https://github.com/Anil7749)
