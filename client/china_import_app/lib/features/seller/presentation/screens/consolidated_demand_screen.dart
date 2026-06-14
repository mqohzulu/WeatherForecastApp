import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/widgets/error_view.dart';
import '../../../../core/widgets/loading_indicator.dart';
import '../../domain/entities/demand_line.dart';
import '../providers/seller_provider.dart';

class ConsolidatedDemandScreen extends ConsumerWidget {
  const ConsolidatedDemandScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final asyncDemand = ref.watch(consolidatedDemandProvider);

    return Scaffold(
      appBar: AppBar(title: const Text('Consolidated Demand')),
      body: asyncDemand.when(
        loading: () => const LoadingIndicator(),
        error: (error, _) => ErrorView(
          message: error.toString(),
          onRetry: () => ref.invalidate(consolidatedDemandProvider),
        ),
        data: (lines) {
          if (lines.isEmpty) {
            return const _EmptyState();
          }
          return RefreshIndicator(
            onRefresh: () async => ref.invalidate(consolidatedDemandProvider),
            child: ListView.separated(
              itemCount: lines.length,
              separatorBuilder: (_, __) => const Divider(height: 1),
              itemBuilder: (context, index) =>
                  _DemandTile(line: lines[index]),
            ),
          );
        },
      ),
    );
  }
}

class _DemandTile extends StatelessWidget {
  const _DemandTile({required this.line});

  final DemandLine line;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return ListTile(
      title: Text(
        line.productName,
        maxLines: 1,
        overflow: TextOverflow.ellipsis,
      ),
      subtitle: Text('${line.orderCount} orders'),
      trailing: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        crossAxisAlignment: CrossAxisAlignment.end,
        children: [
          Text(
            '${line.totalQuantity}',
            style: theme.textTheme.titleMedium,
          ),
          Text(
            'units',
            style: theme.textTheme.bodySmall
                ?.copyWith(color: theme.colorScheme.outline),
          ),
        ],
      ),
    );
  }
}

class _EmptyState extends StatelessWidget {
  const _EmptyState();

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Center(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(
            Icons.insights_outlined,
            size: 56,
            color: theme.colorScheme.outline,
          ),
          const SizedBox(height: 16),
          Text('No demand data yet', style: theme.textTheme.bodyLarge),
        ],
      ),
    );
  }
}
