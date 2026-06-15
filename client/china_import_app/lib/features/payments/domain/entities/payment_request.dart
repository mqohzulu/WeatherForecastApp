/// Domain representation of a payment request for an order.
class PaymentRequest {
  const PaymentRequest({
    required this.id,
    required this.orderId,
    required this.amountZar,
    required this.status,
    required this.createdAt,
    this.reference,
  });

  final String id;
  final String orderId;
  final double amountZar;
  final String status;
  final DateTime createdAt;
  final String? reference;
}
