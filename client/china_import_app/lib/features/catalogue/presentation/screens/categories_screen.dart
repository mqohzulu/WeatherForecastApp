import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/router/routes.dart';
import '../../../../core/widgets/error_view.dart';
import '../../../../core/widgets/loading_indicator.dart';
import '../providers/catalogue_provider.dart';
import '../widgets/category_card.dart';

/// Landing screen showing the catalogue grouped by category.
class CategoriesScreen extends ConsumerWidget {
  const CategoriesScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final categories = ref.watch(categoriesProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('China Import'),
        actions: [
          IconButton(
            tooltip: 'Announcements',
            icon: const Icon(Icons.campaign_outlined),
            onPressed: () => context.push(Routes.announcementsPath),
          ),
          IconButton(
            tooltip: 'My orders',
            icon: const Icon(Icons.receipt_long_outlined),
            onPressed: () => context.push(Routes.ordersPath),
          ),
          IconButton(
            tooltip: 'Cart',
            icon: const Icon(Icons.shopping_cart_outlined),
            onPressed: () => context.push(Routes.cartPath),
          ),
        ],
      ),
      body: categories.when(
        loading: () => const LoadingIndicator(message: 'Loading categories…'),
        error: (error, _) => ErrorView(
          message: error.toString(),
          onRetry: () => ref.invalidate(categoriesProvider),
        ),
        data: (items) {
          if (items.isEmpty) {
            return const ErrorView(
              message: 'No categories available yet.',
              icon: Icons.category_outlined,
            );
          }
          return RefreshIndicator(
            onRefresh: () async => ref.invalidate(categoriesProvider),
            child: GridView.builder(
              padding: const EdgeInsets.all(16),
              gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                crossAxisCount: 2,
                mainAxisSpacing: 16,
                crossAxisSpacing: 16,
                childAspectRatio: 0.82,
              ),
              itemCount: items.length,
              itemBuilder: (context, index) {
                final category = items[index];
                return CategoryCard(
                  category: category,
                  onTap: () => context.push(
                    '${Routes.productListPath}?categoryId=${category.id}',
                  ),
                );
              },
            ),
          );
        },
      ),
    );
  }
}
