import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../domain/entities/cart_item.dart';

// ---------------------------------------------------------------------------
// Cart state
// ---------------------------------------------------------------------------

class CartState {
  const CartState({this.items = const []});

  final List<CartItem> items;

  int get totalQuantity =>
      items.fold(0, (sum, item) => sum + item.quantity);

  double get subtotalZar =>
      items.fold<double>(0, (sum, item) => sum + item.lineTotalZar);

  bool get isEmpty => items.isEmpty;

  CartState copyWith({List<CartItem>? items}) {
    return CartState(items: items ?? this.items);
  }
}

// ---------------------------------------------------------------------------
// Cart notifier
// ---------------------------------------------------------------------------

class CartNotifier extends StateNotifier<CartState> {
  CartNotifier() : super(const CartState());

  /// Adds [item], merging into an existing line with the same key.
  void addItem(CartItem item) {
    final index = state.items.indexWhere((e) => e.lineKey == item.lineKey);
    if (index == -1) {
      state = state.copyWith(items: [...state.items, item]);
      return;
    }
    final existing = state.items[index];
    final updated = [...state.items];
    updated[index] =
        existing.copyWith(quantity: existing.quantity + item.quantity);
    state = state.copyWith(items: updated);
  }

  /// Sets the quantity for [lineKey]; removes the line when [quantity] <= 0.
  void setQuantity(String lineKey, int quantity) {
    if (quantity <= 0) {
      removeItem(lineKey);
      return;
    }
    final index = state.items.indexWhere((e) => e.lineKey == lineKey);
    if (index == -1) return;
    final updated = [...state.items];
    updated[index] = updated[index].copyWith(quantity: quantity);
    state = state.copyWith(items: updated);
  }

  void increment(String lineKey) {
    final index = state.items.indexWhere((e) => e.lineKey == lineKey);
    if (index == -1) return;
    setQuantity(lineKey, state.items[index].quantity + 1);
  }

  void decrement(String lineKey) {
    final index = state.items.indexWhere((e) => e.lineKey == lineKey);
    if (index == -1) return;
    setQuantity(lineKey, state.items[index].quantity - 1);
  }

  void removeItem(String lineKey) {
    state = state.copyWith(
      items: state.items.where((e) => e.lineKey != lineKey).toList(),
    );
  }

  void clear() {
    state = const CartState();
  }
}

final cartProvider = StateNotifierProvider<CartNotifier, CartState>((ref) {
  return CartNotifier();
});

/// Total number of items across all lines — handy for badges.
final cartCountProvider = Provider<int>((ref) {
  return ref.watch(cartProvider).totalQuantity;
});
