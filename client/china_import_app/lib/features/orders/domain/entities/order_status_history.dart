import 'order_status.dart';

/// A single transition in an order's status lifecycle.
class OrderStatusHistory {
  const OrderStatusHistory({
    required this.status,
    required this.timestamp,
    this.note,
  });

  final OrderStatus status;
  final DateTime timestamp;
  final String? note;
}
