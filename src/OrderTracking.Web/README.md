# Order Tracking Web - Frontend

Frontend do sistema de rastreamento de pedidos desenvolvido com **React + TypeScript + Vite**.

## 🚀 Tecnologias

- **React 18.3** - Biblioteca JavaScript para construção de interfaces
- **TypeScript 5.2** - Superset JavaScript com tipagem estática
- **Vite 5.2** - Build tool moderna e ultra-rápida
- **Axios** - Cliente HTTP para requisições à API

## 📋 Funcionalidades

### ✅ Implementadas conforme desafio:

1. **Cadastrar Pedido** 
   - Formulário para enviar requisição `POST /orders`
   - Validação de campos
   - Feedback visual de sucesso/erro

2. **Listar Pedidos**
   - Consome endpoint `GET /orders`
   - Exibe detalhes: ID, Cliente, Valor, Data, Status
   - Auto-refresh a cada 5 segundos
   - Formatação de moeda (BRL) e data (pt-BR)

## 🛠️ Instalação e Execução

### Pré-requisitos

- Node.js 18+ (ou versão LTS mais recente)
- npm ou yarn

### 1. Instalar dependências

```bash
npm install
```

### 2. Configurar variáveis de ambiente

Copie o arquivo `.env.example` para `.env`:

```bash
cp .env.example .env
```

Edite o arquivo `.env` se necessário:

```env
VITE_API_URL=http://localhost:5000
```

### 3. Executar em modo desenvolvimento

```bash
npm run dev
# ou
npm start
```

A aplicação estará disponível em: **http://localhost:3000**

### 4. Build para produção

```bash
npm run build
```

Os arquivos otimizados estarão na pasta `dist/`.

### 5. Preview do build de produção

```bash
npm run preview
```

## 📁 Estrutura do Projeto

```
src/
├── components/           # Componentes React
│   ├── OrderForm.tsx    # Formulário de cadastro
│   ├── OrderForm.css    # Estilos do formulário
│   ├── OrderList.tsx    # Lista de pedidos
│   └── OrderList.css    # Estilos da lista
├── services/            # Camada de serviços
│   └── orderService.ts  # Integração com API
├── App.tsx              # Componente principal
├── App.css              # Estilos globais da aplicação
├── main.tsx             # Entry point
└── index.css            # Reset CSS e estilos base
```

## 🔧 Scripts Disponíveis

| Script | Descrição |
|--------|-----------|
| `npm run dev` | Inicia servidor de desenvolvimento (porta 3000) |
| `npm start` | Alias para `npm run dev` |
| `npm run build` | Compila e gera build de produção |
| `npm run preview` | Preview do build de produção |

## 🌐 Integração com Backend

A aplicação se comunica com a API .NET através dos seguintes endpoints:

- `POST /api/orders` - Criar novo pedido
- `GET /api/orders` - Listar todos os pedidos
- `GET /api/orders/{id}` - Buscar pedido por ID

### Configuração de Proxy (Desenvolvimento)

O Vite está configurado para fazer proxy das requisições `/api` para `http://localhost:5000` automaticamente em desenvolvimento. Veja `vite.config.ts`.

## ⚡ Vantagens do Vite sobre CRA

- **Start instantâneo**: HMR (Hot Module Replacement) extremamente rápido
- **Build otimizado**: Usa esbuild e Rollup para builds de produção
- **Menor bundle size**: Código mais enxuto e otimizado
- **Suporte nativo a ESM**: Módulos ES nativos
- **TypeScript out-of-the-box**: Sem configuração adicional

## 🐛 Troubleshooting

### Porta 3000 já em uso

Edite `vite.config.ts` e altere a porta:

```typescript
server: {
  port: 3001, // Altere para porta desejada
}
```

### API não responde

1. Certifique-se de que a API está rodando em `http://localhost:5000`
2. Verifique o arquivo `.env` e a variável `VITE_API_URL`
3. Verifique os logs do navegador (F12 → Console)

### Erro de CORS

A API .NET deve estar configurada com CORS habilitado para aceitar requisições do frontend.

## 📝 Observações

- O projeto usa **auto-refresh** na lista de pedidos (atualiza a cada 5 segundos)
- Todos os valores monetários são formatados em **R$ (BRL)**
- Datas formatadas no padrão brasileiro (**dd/MM/yyyy**)
- Interface totalmente em **português**

## 🔗 Links Úteis

- [Documentação Vite](https://vitejs.dev/)
- [Documentação React](https://react.dev/)
- [Documentação TypeScript](https://www.typescriptlang.org/)
- [Documentação Axios](https://axios-http.com/)

---

Desenvolvido como parte do **Teste Prático – Desenvolvedor .NET**
