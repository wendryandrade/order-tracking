import { useState, FC } from 'react';
import { orderService, CreateOrderRequest } from '../services/orderService';
import './OrderForm.css';

interface OrderFormProps {
  onOrderCreated: () => void;
}

export const OrderForm: FC<OrderFormProps> = ({ onOrderCreated }) => {
  const [customerName, setCustomerName] = useState('');
  const [amount, setAmount] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  // Data atual para exibição (sempre atualizada)
  const getCurrentDate = () => new Date().toISOString().split('T')[0];

  // Função para garantir precisão decimal
  const parseDecimalAmount = (value: string): number => {
    // Remove caracteres não numéricos exceto ponto e vírgula
    const cleanValue = value.replace(',', '.');
    // Converte para número com 2 casas decimais
    const numValue = parseFloat(cleanValue);
    // Arredonda para 2 casas decimais
    return Math.round(numValue * 100) / 100;
  };

  // Handler para input de valor com validação
  const handleAmountChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    // Permite apenas números e ponto decimal
    if (value === '' || /^\d*\.?\d{0,2}$/.test(value)) {
      setAmount(value);
    }
  };

  // Handler para input de nome (apenas letras e espaços)
  const handleNameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    // Permite apenas letras, espaços e acentos
    if (value === '' || /^[a-zA-ZÀ-ÿ\s]*$/.test(value)) {
      setCustomerName(value);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSuccess('');
    setLoading(true);

    try {
      // Garantir precisão decimal correta
      const amountValue = parseDecimalAmount(amount);

      const request: CreateOrderRequest = {
        customerName,
        amount: amountValue,
        orderDate: new Date().toISOString(), // Sempre a data/hora atual
      };

      const response = await orderService.createOrder(request);
      setSuccess(`Pedido ${response.id} criado com sucesso! ${response.message}`);

      // Limpar formulário
      setCustomerName('');
      setAmount('');

      // Notificar componente pai
      onOrderCreated();
    } catch (err: any) {
      setError(err.response?.data?.error || 'Erro ao criar pedido. Tente novamente.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="order-form-container">
      <h2>Cadastrar Novo Pedido</h2>
      <form onSubmit={handleSubmit} className="order-form">
        <div className="form-group">
          <label htmlFor="customerName">Nome do Cliente *</label>
          <input
            type="text"
            id="customerName"
            value={customerName}
            onChange={handleNameChange}
            required
            minLength={3}
            maxLength={200}
            placeholder="Digite o nome do cliente (apenas letras)"
            disabled={loading}
          />
        </div>

        <div className="form-group">
          <label htmlFor="amount">Valor do Pedido (R$) *</label>
          <input
            type="text"
            inputMode="decimal"
            id="amount"
            value={amount}
            onChange={handleAmountChange}
            required
            placeholder="10.00"
            disabled={loading}
          />
        </div>

        <div className="form-group">
          <label htmlFor="orderDate">Data do Pedido (automática)</label>
          <input
            type="date"
            id="orderDate"
            value={getCurrentDate()}
            disabled
            className="readonly-field"
          />
          <small className="field-hint">A data é definida automaticamente no momento do cadastro</small>
        </div>

        {error && <div className="alert alert-error">{error}</div>}
        {success && <div className="alert alert-success">{success}</div>}

        <button type="submit" disabled={loading} className="btn-submit">
          {loading ? 'Enviando...' : 'Cadastrar Pedido'}
        </button>
      </form>
    </div>
  );
};
