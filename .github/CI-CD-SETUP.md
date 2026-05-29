# 🚀 Guia de CI/CD - GitHub Actions + Docker

Este projeto possui pipelines automatizados de **CI/CD** usando GitHub Actions para build, testes e publicação de imagens Docker.

---

## 📋 **Workflows Disponíveis**

### 1️⃣ **CI - Build & Test** (`.github/workflows/ci.yml`)
Executa em **Pull Requests**:
- ✅ Análise de qualidade de código (.NET Format)
- ✅ Build do backend (.NET 10)
- ✅ Testes unitários com cobertura de código
- ✅ Build do frontend (Vite + React)
- ✅ Validação dos Dockerfiles

**Triggers:**
- Pull Requests para `main` ou `develop`

---

### 2️⃣ **CI/CD Pipeline** (`.github/workflows/ci-cd.yml`)
Pipeline completo de **Continuous Integration e Deployment**:

#### **Stage 1: Build & Test Backend**
- Restore, Build e Testes do .NET
- Publicação de relatórios de teste

#### **Stage 2: Build Frontend**
- Build do React/Vite
- Otimização de bundle

#### **Stage 3: Build & Push Docker Images**
- Build multi-arquitetura (amd64 + arm64)
- Push para Docker Hub
- Tags automáticas: `latest`, `main-sha`, `branch`
- Cache otimizado via GitHub Actions

**Triggers:**
- Push para `main` ou `develop`
- Pull Requests para `main`
- Manual (workflow_dispatch)

---

## 🔐 **Configuração de Secrets**

Para o pipeline funcionar, configure os seguintes **secrets** no GitHub:

### **Acesso ao Repositório:**
```
Settings → Secrets and variables → Actions → New repository secret
```

### **Secrets Necessários:**

#### **Docker Hub:**
```
DOCKER_USERNAME        # Seu username do Docker Hub
DOCKER_PASSWORD        # Token de acesso do Docker Hub (não use sua senha)
```

#### **Variáveis de Ambiente (Opcional):**
```
VITE_API_URL    # URL da API em produção (ex: http://seu-servidor:5000)
```

---

## 🐳 **Como obter Docker Hub Access Token**

1. Acesse [Docker Hub](https://hub.docker.com/)
2. Vá em **Account Settings → Security → New Access Token**
3. Nome: `GitHub Actions OrderTracking`
4. Permissões: `Read, Write, Delete`
5. Copie o token (ele só aparece uma vez!)
6. Cole no secret `DOCKER_PASSWORD` do GitHub

---

## 📦 **Imagens Docker Geradas**

Após o push para `main`, as seguintes imagens são publicadas:

```
docker.io/SEU_USERNAME/ordertracking-api:latest
docker.io/SEU_USERNAME/ordertracking-worker:latest
docker.io/SEU_USERNAME/ordertracking-web:latest
```

**Tags disponíveis:**
- `latest` - Última versão da branch main
- `main-abc1234` - Commit SHA específico
- `v1.0.0` - Tags semver (se você criar release tags)

---

## 🎯 **Fluxo de Deploy**

### **Desenvolvimento (develop branch):**
```
1. Push/PR para develop
   ↓
2. CI: Build + Testes + Validação Docker
   ↓
3. ✅ Validação bem-sucedida (sem push de imagens)
```

### **Produção (main branch):**
```
1. Merge para main
   ↓
2. CI: Build + Testes
   ↓
3. Build Docker Images (multi-arch)
   ↓
4. Push para Docker Hub
   ↓
5. ✅ Imagens publicadas
```

---

## 🚀 **Como usar as imagens Docker**

### **1. Atualizar docker-compose.prod.yml:**

```yaml
services:
  api:
    image: SEU_USERNAME/ordertracking-api:latest

  worker:
    image: SEU_USERNAME/ordertracking-worker:latest

  web:
    image: SEU_USERNAME/ordertracking-web:latest
```

### **2. Deploy em seu servidor:**

```bash
# Pull das imagens mais recentes
docker-compose -f docker-compose.prod.yml pull

# Restart dos containers
docker-compose -f docker-compose.prod.yml up -d

# Ver logs
docker-compose -f docker-compose.prod.yml logs -f
```

### **3. Deploy automatizado via SSH:**

Crie um script `deploy.sh` no servidor:

```bash
#!/bin/bash
cd /opt/ordertracking
docker-compose -f docker-compose.prod.yml pull
docker-compose -f docker-compose.prod.yml up -d --remove-orphans
docker image prune -f
echo "✅ Deploy concluído!"
```

---

## 🔄 **Deploy Contínuo (Opcional)**

Se quiser deploy automático após build, adicione no workflow:

```yaml
  deploy:
    name: Deploy to Server
    runs-on: ubuntu-latest
    needs: docker-build-push
    if: github.ref == 'refs/heads/main'

    steps:
    - name: 🚀 Deploy via SSH
      uses: appleboy/ssh-action@v1.0.0
      with:
        host: ${{ secrets.SERVER_HOST }}
        username: ${{ secrets.SERVER_USER }}
        key: ${{ secrets.SERVER_SSH_KEY }}
        script: |
          cd /opt/ordertracking
          ./deploy.sh
```

**Secrets adicionais necessários:**
```
SERVER_HOST       # IP ou hostname do servidor
SERVER_USER       # Username SSH
SERVER_SSH_KEY    # Chave privada SSH
```

---

## 📊 **Monitoramento**

### **Status Badges:**

Adicione no `README.md`:

```markdown
![CI/CD Pipeline](https://github.com/wendryandrade/order-tracking/actions/workflows/ci-cd.yml/badge.svg)
![CI - Build & Test](https://github.com/wendryandrade/order-tracking/actions/workflows/ci.yml/badge.svg)
![Docker Pulls](https://img.shields.io/docker/pulls/SEU_USERNAME/ordertracking-api)
```

### **Logs:**
- Acesse: `Actions` → Selecione um workflow run
- Visualize logs detalhados de cada job

---

## 🐛 **Troubleshooting**

### **Erro: "Docker login failed"**
- Verifique `DOCKER_USERNAME` e `DOCKER_PASSWORD`
- Certifique-se de usar um **Access Token**, não a senha
- Token deve ter permissões de Read, Write

### **Erro: "Failed to push image"**
- Verifique se o repositório existe no Docker Hub
- Repositórios públicos são criados automaticamente no primeiro push
- Para repositórios privados, crie manualmente antes

### **Erro: "Tests failed"**
- Rode localmente: `dotnet test`
- Verifique logs no Actions

### **Erro: "Frontend build failed"**
- Valide `VITE_API_URL` se configurado
- Rode localmente: `npm run build`

### **Erro: "Context exceeded"**
- Dockerfile está copiando arquivos desnecessários
- Verifique `.dockerignore`

---

## 📚 **Recursos**

- [GitHub Actions Docs](https://docs.github.com/actions)
- [Docker Build Push Action](https://github.com/docker/build-push-action)
- [Docker Hub](https://hub.docker.com/)
- [Multi-platform builds](https://docs.docker.com/build/building/multi-platform/)

---

## ✅ **Checklist de Setup**

- [ ] Criar conta no Docker Hub (gratuita)
- [ ] Criar Access Token no Docker Hub
- [ ] Configurar `DOCKER_USERNAME` no GitHub Secrets
- [ ] Configurar `DOCKER_PASSWORD` no GitHub Secrets
- [ ] (Opcional) Configurar `VITE_API_URL` para produção
- [ ] Fazer push para `main` e verificar o Actions
- [ ] Verificar se imagens foram publicadas no Docker Hub
- [ ] Atualizar `docker-compose.prod.yml` com suas imagens
- [ ] Testar pull e deploy das imagens
- [ ] Adicionar badges no README

---

## 🎉 **Pronto!**

Seu pipeline de CI/CD está configurado! 

A cada push na `main`:
1. ✅ Testes executados
2. 🐳 Imagens Docker criadas
3. 📦 Publicadas no Docker Hub
4. 🚀 Prontas para deploy
