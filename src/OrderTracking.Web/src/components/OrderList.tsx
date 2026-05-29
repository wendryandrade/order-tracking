import { useState, useEffect, FC } from 'react';
import { orderService, OrderResponse } from '../services/orderService';
import './OrderList.css';

interface OrderListProps {
  refreshTrigger: number;
}

export const OrderList: FC<OrderListProps> = ({ refreshTrigger }) => {
  const [orders, setOrders] = useState<OrderResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const fetchOrders = async () => {
    try {
      setLoading(true);
      setError('');

      const data = await orderService.getAllOrders();
      setOrders(data);
    } catch (err: any) {
      setError(`Erro ao carregar pedidos: ${err.message || 'Verifique se a API está rodando'}`);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchOrders();
  }, [refreshTrigger]);

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(value);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    });
  };

  const getStatusBadgeClass = (status: string) => {
    switch (status.toLowerCase()) {
      case 'pending':
        return 'status-badge status-pending';
      case 'processed':
        return 'status-badge status-processed';
      default:
        return 'status-badge';
    }
  };

  const getStatusLabel = (status: string) => {
    switch (status.toLowerCase()) {
      case 'pending':
        return 'Pendente';
      case 'processed':
        return 'Processado';
      default:
        return status;
    }
  };

  if (loading && orders.length === 0) {
    return (
      <div className="order-list-container">
        <h2>Lista de Pedidos</h2>
        <div className="loading">Carregando pedidos...</div>
      </div>
    );
  }

  return (
    <div className="order-list-container">
      <div className="order-list-header">
        <h2>Lista de Pedidos</h2>
        <button 
          className="refresh-button" 
          onClick={fetchOrders}
          disabled={loading}
        >
          <svg 
            className="refresh-icon" 
            width="16" 
            height="16" 
            viewBox="0 0 24 24" 
            fill="none" 
            stroke="currentColor" 
            strokeWidth="2" 
            strokeLinecap="round" 
            strokeLinejoin="round"
          >
            <polyline points="23 4 23 10 17 10"></polyline>
            <polyline points="1 20 1 14 7 14"></polyline>
            <path d="M3.51 9a9 9 0 0 1 14.85-3.36L23 10M1 14l4.64 4.36A9 9 0 0 0 20.49 15"></path>
          </svg>
          {loading ? 'Atualizando...' : 'Atualizar'}
        </button>
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      {orders.length === 0 ? (
        <div className="empty-state">
          <p>Nenhum pedido cadastrado ainda.</p>
          <p>Cadastre seu primeiro pedido usando o formulário acima!</p>
        </div>
      ) : (
        <div className="table-container">
          <table className="orders-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Cliente</th>
                <th>Valor</th>
                <th>Data do Pedido</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {orders.map((order) => (
                <tr key={order.id}>
                  <td className="order-id">{order.id.substring(0, 8)}...</td>
                  <td>{order.customerName}</td>
                  <td className="order-amount">{formatCurrency(order.amount)}</td>
                  <td>{formatDate(order.orderDate)}</td>
                  <td>
                    <span className={getStatusBadgeClass(order.status)}>
                      {getStatusLabel(order.status)}
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <div className="order-count">
        Total: {orders.length} pedido{orders.length !== 1 ? 's' : ''}
      </div>
    </div>
  );
};
