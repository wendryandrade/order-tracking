import { useState } from 'react';
import { OrderForm } from './components/OrderForm';
import { OrderList } from './components/OrderList';
import './App.css';

function App() {
  const [refreshTrigger, setRefreshTrigger] = useState(0);

  const handleOrderCreated = () => {
    // Incrementar trigger para forçar atualização da lista
    setRefreshTrigger(prev => prev + 1);
  };

  return (
    <div className="App">
      <header className="App-header">
        <h1>🚚 Order Tracking</h1>
        <p>Sistema de Rastreamento de Pedidos com Mensageria</p>
      </header>

      <main className="App-main">
        <OrderForm onOrderCreated={handleOrderCreated} />
        <OrderList refreshTrigger={refreshTrigger} />
      </main>

      <footer className="App-footer">
        <p>
          Desenvolvido com .NET 10, SQL Server, RabbitMQ e React/Vite
        </p>
      </footer>
    </div>
  );
}

export default App;
