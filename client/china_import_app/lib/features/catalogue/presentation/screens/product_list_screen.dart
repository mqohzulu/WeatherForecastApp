import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/router/routes.dart';
import '../../../../core/widgets/error_view.dart';
import '../../../../core/widgets/loading_indicator.dart';
import '../providers/catalogue_provider.dart';
import '../widgets/product_card.dart';

/// Grid of products, optionally scoped to a single [categoryId].
class ProductListScreen extends ConsumerStatefulWidget {
  const ProductListScreen({super.key, this.categoryId});

  final String? categoryId;

  @override
  ConsumerState<ProductListScreen> createState() => _ProductListScreenState();
}

class _ProductListScreenState extends ConsumerState<ProductListScreen> {
  final _searchController = TextEditingController();

  @override
  void initState() {
    super.initState();
    _searchController.text = ref.read(productSearchQueryProvider);
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  void _onSearchSubmitted(String value) {
    ref.read(productSearchQueryProvider.notifier).state = value.trim();
  }

  @override
  Widget build(BuildContext context) {
    final products = ref.watch(productsProvider(widget.categoryId));

    return Scaffold(
      appBar: AppBar(
        title: const Text('Products'),
        actions: [
          IconButton(
            tooltip: 'Cart',
            icon: const Icon(Icons.shopping_cart_outlined),
            onPressed: () => context.push(Routes.cartPath),
          ),
        ],
        bottom: PreferredSize(
          preferredSize: const Size.fromHeight(64),
          child: Padding(
            padding: const EdgeInsets.fromLTRB(16, 0, 16, 12),
            child: TextField(
              controller: _searchController,
              textInputAction: TextInputAction.search,
              onSubmitted: _onSearchSubmitted,
              decoration: InputDecoration(
                hintText: 'Search products',
                prefixIcon: const Icon(Icons.search),
                suffixIcon: IconButton(
                  icon: const Icon(Icons.clear),
                  onPressed: () {
                    _searchController.clear();
                    _onSearchSubmitted('');
                  },
                ),
                isDense: true,
                filled: true,
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                  borderSide: BorderSide.none,
                ),
              ),
            ),
          ),
        ),
      ),
      body: products.when(
        loading: () => const LoadingIndicator(message: 'Loading products…'),
        error: (error, _) => ErrorView(
          message: error.toString(),
          onRetry: () => ref.invalidate(productsProvider(widget.categoryId)),
        ),
        data: (page) {
          if (page.isEmpty) {
            return const ErrorView(
              message: 'No products found.',
              icon: Icons.inventory_2_outlined,
            );
          }
          return RefreshIndicator(
            onRefresh: () async =>
                ref.invalidate(productsProvider(widget.categoryId)),
            child: GridView.builder(
              padding: const EdgeInsets.all(16),
              gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                crossAxisCount: 2,
                mainAxisSpacing: 16,
                crossAxisSpacing: 16,
                childAspectRatio: 0.7,
              ),
              itemCount: page.items.length,
              itemBuilder: (context, index) {
                final product = page.items[index];
                return ProductCard(
                  product: product,
                  onTap: () => context.push(
                    Routes.productDetailLocation(product.id),
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
