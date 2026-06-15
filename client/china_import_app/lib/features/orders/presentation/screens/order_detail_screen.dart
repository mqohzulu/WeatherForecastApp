import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/utils/formatters.dart';
import '../../../../core/widgets/error_view.dart';
import '../../../../core/widgets/loading_indicator.dart';
import '../../domain/entities/order.dart';
import '../../domain/entities/order_item.dart';
import '../providers/orders_provider.dart';
import '../widgets/order_status_badge.dart';
import '../widgets/order_status_timeline.dart';

/// Shows the full detail of a single order.
class OrderDetailScreen extends ConsumerWidget {
  const OrderDetailScreen({super.key, required this.orderId});

  final String orderId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final orderAsync = ref.watch(orderDetailProvider(orderId));

    return Scaffold(
      appBar: AppBar(title: const Text('Order')),
      body: orderAsync.when(
        loading: () => const LoadingIndicator(),
        error: (error, _) => ErrorView(
          message: error.toString(),
          onRetry: () => ref.invalidate(orderDetailProvider(orderId)),
        ),
        data: (order) => _OrderDetailBody(order: order),
      ),
    );
  }
}

class _OrderDetailBody extends StatelessWidget {
  const _OrderDetailBody({required this.order});

  final Order order;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Row(
          children: [
            Expanded(
              child: Text(
                order.reference,
                style: theme.textTheme.titleLarge?.copyWith(
                  fontWeight: FontWeight.w700,
                ),
              ),
            ),
            OrderStatusBadge(status: order.status),
          ],
        ),
        const SizedBox(height: 4),
        Text(
          'Placed ${Formatters.date(order.createdAt)}',
          style: theme.textTheme.bodySmall?.copyWith(
            color: theme.colorScheme.outline,
          ),
        ),
        const SizedBox(height: 24),
        Text('Progress', style: theme.textTheme.titleMedium),
        const SizedBox(height: 8),
        OrderStatusTimeline(order: order),
        const SizedBox(height: 24),
        Text('Items', style: theme.textTheme.titleMedium),
        const SizedBox(height: 8),
        Card(
          clipBehavior: Clip.antiAlias,
          child: Column(
            children: [
              for (var i = 0; i < order.items.length; i++) ...[
                if (i > 0) const Divider(height: 1),
                _OrderItemTile(item: order.items[i]),
              ],
            ],
          ),
        ),
        const SizedBox(height: 16),
        if (order.notes != null && order.notes!.isNotEmpty) ...[
          Text('Notes', style: theme.textTheme.titleMedium),
          const SizedBox(height: 8),
          Text(order.notes!, style: theme.textTheme.bodyMedium),
          const SizedBox(height: 16),
        ],
        const Divider(),
        Padding(
          padding: const EdgeInsets.symmetric(vertical: 8),
          child: Row(
            children: [
              Text('Total', style: theme.textTheme.titleMedium),
              const Spacer(),
              Text(
                Formatters.zar(order.totalZar),
                style: theme.textTheme.titleLarge?.copyWith(
                  fontWeight: FontWeight.w700,
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _OrderItemTile extends StatelessWidget {
  const _OrderItemTile({required this.item});

  final OrderItem item;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Padding(
      padding: const EdgeInsets.all(16),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  item.productName,
                  style: theme.textTheme.bodyLarge?.copyWith(
                    fontWeight: FontWeight.w600,
                  ),
                ),
                if (item.variantLabel != null &&
                    item.variantLabel!.isNotEmpty) ...[
                  const SizedBox(height: 2),
                  Text(
                    item.variantLabel!,
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: theme.colorScheme.outline,
                    ),
                  ),
                ],
                const SizedBox(height: 4),
                Text(
                  '${item.quantity} x ${Formatters.zar(item.unitPriceZar)}',
                  style: theme.textTheme.bodyMedium,
                ),
              ],
            ),
          ),
          const SizedBox(width: 12),
          Text(
            Formatters.zar(item.lineTotalZar),
            style: theme.textTheme.bodyLarge?.copyWith(
              fontWeight: FontWeight.w600,
            ),
          ),
        ],
      ),
    );
  }
}
