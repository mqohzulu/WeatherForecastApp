import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/utils/formatters.dart';
import '../../domain/entities/order.dart';
import '../../domain/entities/order_status.dart';
import '../../domain/entities/order_status_history.dart';
import 'order_status_badge.dart';

/// Canonical 'happy path' progression rendered by the timeline.
const List<OrderStatus> _happyPath = [
  OrderStatus.placed,
  OrderStatus.confirmed,
  OrderStatus.beingSourced,
  OrderStatus.shipped,
  OrderStatus.arrivedInSa,
  OrderStatus.readyForCollection,
  OrderStatus.completed,
];

/// A self-contained vertical timeline of an order's progress.
///
/// Each happy-path step is marked done if it appears in the order history or
/// is at/before the current status. When the order ended negatively
/// (cancelled/rejected) a distinct final node is appended.
class OrderStatusTimeline extends StatelessWidget {
  const OrderStatusTimeline({super.key, required this.order});

  final Order order;

  /// The most recent history entry for [status], if any.
  OrderStatusHistory? _historyFor(OrderStatus status) {
    OrderStatusHistory? match;
    for (final entry in order.history) {
      if (entry.status == status) {
        if (match == null || entry.timestamp.isAfter(match.timestamp)) {
          match = entry;
        }
      }
    }
    return match;
  }

  @override
  Widget build(BuildContext context) {
    final negative = order.status.isNegative;
    final currentIndex = _happyPath.indexOf(order.status);

    final steps = <Widget>[];
    for (var i = 0; i < _happyPath.length; i++) {
      final status = _happyPath[i];
      final history = _historyFor(status);
      final reached = history != null ||
          (currentIndex >= 0 && i <= currentIndex && !negative);
      steps.add(
        _TimelineNode(
          color: reached ? orderStatusColor(status) : AppColors.outline,
          filled: reached,
          isFirst: i == 0,
          isLast: i == _happyPath.length - 1 && !negative,
          label: status.label,
          done: reached,
          timestamp: history?.timestamp,
          note: history?.note,
        ),
      );
    }

    if (negative) {
      final history = _historyFor(order.status);
      steps.add(
        _TimelineNode(
          color: orderStatusColor(order.status),
          filled: true,
          isFirst: false,
          isLast: true,
          label: order.status.label,
          done: true,
          timestamp: history?.timestamp,
          note: history?.note,
        ),
      );
    }

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: steps,
    );
  }
}

class _TimelineNode extends StatelessWidget {
  const _TimelineNode({
    required this.color,
    required this.filled,
    required this.isFirst,
    required this.isLast,
    required this.label,
    required this.done,
    this.timestamp,
    this.note,
  });

  final Color color;
  final bool filled;
  final bool isFirst;
  final bool isLast;
  final String label;
  final bool done;
  final DateTime? timestamp;
  final String? note;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return IntrinsicHeight(
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Column(
            children: [
              Expanded(
                child: Container(
                  width: 2,
                  color: isFirst
                      ? Colors.transparent
                      : color.withValues(alpha: 0.4),
                ),
              ),
              CircleAvatar(
                radius: 9,
                backgroundColor: filled
                    ? color
                    : theme.colorScheme.surface,
                child: CircleAvatar(
                  radius: 5,
                  backgroundColor: filled ? color : AppColors.outline,
                ),
              ),
              Expanded(
                child: Container(
                  width: 2,
                  color: isLast
                      ? Colors.transparent
                      : color.withValues(alpha: 0.4),
                ),
              ),
            ],
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Padding(
              padding: const EdgeInsets.symmetric(vertical: 8),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    label,
                    style: theme.textTheme.bodyMedium?.copyWith(
                      fontWeight: done ? FontWeight.w600 : FontWeight.w400,
                      color: done
                          ? theme.colorScheme.onSurface
                          : AppColors.textSecondary,
                    ),
                  ),
                  if (timestamp != null)
                    Text(
                      Formatters.dateTime(timestamp!),
                      style: theme.textTheme.bodySmall?.copyWith(
                        color: AppColors.textSecondary,
                      ),
                    ),
                  if (note != null && note!.isNotEmpty)
                    Padding(
                      padding: const EdgeInsets.only(top: 2),
                      child: Text(
                        note!,
                        style: theme.textTheme.bodySmall,
                      ),
                    ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}
