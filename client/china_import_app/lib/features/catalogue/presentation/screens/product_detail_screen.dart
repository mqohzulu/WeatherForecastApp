import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/router/routes.dart';
import '../../../../core/utils/formatters.dart';
import '../../../../core/widgets/error_view.dart';
import '../../../../core/widgets/loading_indicator.dart';
import '../../domain/entities/product.dart';
import '../../domain/entities/product_variant.dart';
import '../providers/catalogue_provider.dart';

/// Full product detail with image carousel, variants and add-to-cart action.
class ProductDetailScreen extends ConsumerWidget {
  const ProductDetailScreen({super.key, required this.productId});

  final String productId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final product = ref.watch(productDetailProvider(productId));

    return Scaffold(
      appBar: AppBar(
        title: const Text('Product'),
        actions: [
          IconButton(
            tooltip: 'Cart',
            icon: const Icon(Icons.shopping_cart_outlined),
            onPressed: () => context.push(Routes.cartPath),
          ),
        ],
      ),
      body: product.when(
        loading: () => const LoadingIndicator(message: 'Loading product…'),
        error: (error, _) => ErrorView(
          message: error.toString(),
          onRetry: () => ref.invalidate(productDetailProvider(productId)),
        ),
        data: (p) => _ProductDetailBody(product: p),
      ),
      bottomNavigationBar: product.maybeWhen(
        data: (p) => SafeArea(
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: FilledButton.icon(
              icon: const Icon(Icons.add_shopping_cart),
              label: const Text('Add to cart'),
              onPressed: () {
                // TODO(cart): wire up the cart provider once available.
                ScaffoldMessenger.of(context)
                  ..hideCurrentSnackBar()
                  ..showSnackBar(
                    const SnackBar(content: Text('Added to cart')),
                  );
              },
            ),
          ),
        ),
        orElse: () => const SizedBox.shrink(),
      ),
    );
  }
}

class _ProductDetailBody extends StatelessWidget {
  const _ProductDetailBody({required this.product});

  final Product product;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return ListView(
      children: [
        _ImageCarousel(imageUrls: product.imageUrls),
        Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(product.name, style: theme.textTheme.headlineSmall),
              const SizedBox(height: 8),
              Text(
                Formatters.zar(product.indicativePriceZar),
                style: theme.textTheme.titleLarge?.copyWith(
                  color: theme.colorScheme.primary,
                  fontWeight: FontWeight.w700,
                ),
              ),
              if (product.description != null &&
                  product.description!.isNotEmpty) ...[
                const SizedBox(height: 20),
                Text('Description', style: theme.textTheme.titleMedium),
                const SizedBox(height: 8),
                Text(product.description!, style: theme.textTheme.bodyMedium),
              ],
              if (product.variants.isNotEmpty) ...[
                const SizedBox(height: 24),
                Text('Variants', style: theme.textTheme.titleMedium),
                const SizedBox(height: 12),
                _VariantList(variants: product.variants),
              ],
            ],
          ),
        ),
      ],
    );
  }
}

class _ImageCarousel extends StatefulWidget {
  const _ImageCarousel({required this.imageUrls});

  final List<String> imageUrls;

  @override
  State<_ImageCarousel> createState() => _ImageCarouselState();
}

class _ImageCarouselState extends State<_ImageCarousel> {
  final _controller = PageController();
  int _current = 0;

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final urls = widget.imageUrls;

    final placeholder = Container(
      color: theme.colorScheme.surfaceContainerHighest,
      alignment: Alignment.center,
      child: Icon(
        Icons.image_outlined,
        size: 56,
        color: theme.colorScheme.outline,
      ),
    );

    if (urls.isEmpty) {
      return AspectRatio(aspectRatio: 1, child: placeholder);
    }

    return Column(
      children: [
        AspectRatio(
          aspectRatio: 1,
          child: PageView.builder(
            controller: _controller,
            onPageChanged: (index) => setState(() => _current = index),
            itemCount: urls.length,
            itemBuilder: (context, index) => CachedNetworkImage(
              imageUrl: urls[index],
              fit: BoxFit.cover,
              placeholder: (context, _) => placeholder,
              errorWidget: (context, _, __) => placeholder,
            ),
          ),
        ),
        if (urls.length > 1) ...[
          const SizedBox(height: 12),
          Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              for (var i = 0; i < urls.length; i++)
                Container(
                  width: 8,
                  height: 8,
                  margin: const EdgeInsets.symmetric(horizontal: 4),
                  decoration: BoxDecoration(
                    shape: BoxShape.circle,
                    color: i == _current
                        ? theme.colorScheme.primary
                        : theme.colorScheme.outlineVariant,
                  ),
                ),
            ],
          ),
        ],
      ],
    );
  }
}

class _VariantList extends StatelessWidget {
  const _VariantList({required this.variants});

  final List<ProductVariant> variants;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Column(
      children: [
        for (final variant in variants)
          Card(
            margin: const EdgeInsets.only(bottom: 8),
            child: ListTile(
              leading: const Icon(Icons.tune),
              title: Text(variant.label),
              trailing: variant.priceZar == null
                  ? null
                  : Text(
                      Formatters.zar(variant.priceZar!),
                      style: theme.textTheme.titleSmall?.copyWith(
                        color: theme.colorScheme.primary,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
            ),
          ),
      ],
    );
  }
}
