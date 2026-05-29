# Stage 1: Build
FROM node:20-alpine AS build
WORKDIR /app

# Argumentos de build
ARG VITE_API_URL=http://localhost:5000
ENV VITE_API_URL=$VITE_API_URL

# Copiar package files
COPY package*.json ./

# Instalar dependências
RUN npm ci --silent

# Copiar código fonte
COPY . .

# Build da aplicação com Vite
RUN npm run build

# Stage 2: Runtime com Nginx
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Remover conteúdo padrão do nginx
RUN rm -rf ./*

# Copiar build da aplicação (Vite gera em /dist)
COPY --from=build /app/dist .

# Copiar configuração customizada do nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
