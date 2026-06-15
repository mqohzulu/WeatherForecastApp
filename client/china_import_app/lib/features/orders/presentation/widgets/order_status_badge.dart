import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';
import '../../domain/entities/order_status.dart';

/// Maps an [OrderStatus] to its brand colour.
Color orderStatusColor(OrderStatus status) {
  switch (status) {
    case OrderStatus.placed:
      return AppColors.statusPlaced;
    case OrderStatus.confirmed:
      return AppColors.statusConfirmed;
    case OrderStatus.beingSourced:
      return AppColors.statusBeingSourced;
    case OrderStatus.shipped:
      return AppColors.statusShipped;
    case OrderStatus.arrivedInSa:
      return AppColors.statusArrivedInSa;
    case OrderStatus.readyForCollection:
      return AppColors.statusReadyForCollection;
    case OrderStatus.completed:
      return AppColors.statusCompleted;
    case OrderStatus.cancelled:
      return AppColors.statusCancelled;
    case OrderStatus.rejected:
      return AppColors.statusRejected;
  }
}

/// A coloured chip showing an order's current status.
class OrderStatusBadge extends StatelessWidget {
  const OrderStatusBadge({super.key, required this.status});

  final OrderStatus status;

  @override
  Widget build(BuildContext context) {
    final color = orderStatusColor(status);
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.12),
        borderRadius: BorderRadius.circular(999),
        border: Border.all(color: color.withValues(alpha: 0.4)),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Container(
            width: 8,
            height: 8,
            decoration: BoxDecoration(
              color: color,
              shape: BoxShape.circle,
            ),
          ),
          const SizedBox(width: 6),
          Text(
            status.label,
            style: Theme.of(context).textTheme.labelMedium?.copyWith(
                  color: color,
                  fontWeight: FontWeight.w600,
                ),
          ),
        ],
      ),
    );
  }
}
