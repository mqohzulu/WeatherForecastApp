import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/router/routes.dart';
import '../../../../core/utils/formatters.dart';
import '../../../../core/widgets/app_button.dart';
import '../providers/cart_provider.dart';
import '../widgets/cart_item_tile.dart';

/// Local cart screen — list of lines, quantity controls and checkout.
class CartScreen extends ConsumerWidget {
  const CartScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(cartProvider);
    final notifier = ref.read(cartProvider.notifier);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Cart'),
        actions: [
          if (!state.isEmpty)
            IconButton(
              onPressed: notifier.clear,
              icon: const Icon(Icons.delete_sweep_outlined),
              tooltip: 'Clear cart',
            ),
        ],
      ),
      body: state.isEmpty
          ? const _EmptyCart()
          : ListView.separated(
              padding: const EdgeInsets.symmetric(vertical: 8),
              itemCount: state.items.length,
              separatorBuilder: (context, index) => const Divider(height: 1),
              itemBuilder: (context, index) {
                final item = state.items[index];
                return CartItemTile(
                  item: item,
                  onIncrement: () => notifier.increment(item.lineKey),
                  onDecrement: () => notifier.decrement(item.lineKey),
                  onRemove: () => notifier.removeItem(item.lineKey),
                );
              },
            ),
      bottomNavigationBar:
          state.isEmpty ? null : _SummaryBar(subtotalZar: state.subtotalZar),
    );
  }
}

class _EmptyCart extends StatelessWidget {
  const _EmptyCart();

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(
              Icons.shopping_cart_outlined,
              size: 56,
              color: theme.colorScheme.onSurfaceVariant,
            ),
            const SizedBox(height: 16),
            Text(
              'Your cart is empty',
              textAlign: TextAlign.center,
              style: theme.textTheme.titleMedium,
            ),
            const SizedBox(height: 8),
            Text(
              'Browse our catalogue and add products to get started.',
              textAlign: TextAlign.center,
              style: theme.textTheme.bodyMedium?.copyWith(
                color: theme.colorScheme.onSurfaceVariant,
              ),
            ),
            const SizedBox(height: 24),
            AppButton(
              label: 'Browse products',
              expanded: false,
              onPressed: () => context.go(Routes.homePath),
            ),
          ],
        ),
      ),
    );
  }
}

class _SummaryBar extends ConsumerWidget {
  const _SummaryBar({required this.subtotalZar});

  final double subtotalZar;

  void _placeOrder(BuildContext context, WidgetRef ref) {
    // TODO: wire to OrdersRepository.createOrder
    ref.read(cartProvider.notifier).clear();
    ScaffoldMessenger.of(context)
      ..hideCurrentSnackBar()
      ..showSnackBar(const SnackBar(content: Text('Order submitted')));
    context.go(Routes.ordersPath);
  }

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final theme = Theme.of(context);
    return Material(
      elevation: 8,
      color: theme.colorScheme.surface,
      child: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text('Subtotal', style: theme.textTheme.titleMedium),
                  Text(
                    Formatters.zar(subtotalZar),
                    style: theme.textTheme.titleMedium,
                  ),
                ],
              ),
              const SizedBox(height: 12),
              AppButton(
                label: 'Place order',
                onPressed: () => _placeOrder(context, ref),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
