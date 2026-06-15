/// Lifecycle states an [Order] can move through.
///
/// The declaration order matches the canonical 'happy path' progression and is
/// relied upon for timeline rendering (later values are 'further along').
enum OrderStatus {
  placed,
  confirmed,
  beingSourced,
  shipped,
  arrivedInSa,
  readyForCollection,
  completed,
  cancelled,
  rejected;

  /// Human-friendly display label.
  String get label {
    switch (this) {
      case OrderStatus.placed:
        return 'Placed';
      case OrderStatus.confirmed:
        return 'Confirmed';
      case OrderStatus.beingSourced:
        return 'Being Sourced';
      case OrderStatus.shipped:
        return 'Shipped';
      case OrderStatus.arrivedInSa:
        return 'Arrived in SA';
      case OrderStatus.readyForCollection:
        return 'Ready for Collection';
      case OrderStatus.completed:
        return 'Completed';
      case OrderStatus.cancelled:
        return 'Cancelled';
      case OrderStatus.rejected:
        return 'Rejected';
    }
  }

  /// Parses an API value, tolerant of camelCase or PascalCase.
  ///
  /// Falls back to [OrderStatus.placed] when the value is unrecognised.
  static OrderStatus fromJson(String value) {
    final normalised = value.isEmpty
        ? value
        : '${value[0].toLowerCase()}${value.substring(1)}';
    for (final status in OrderStatus.values) {
      if (status.name == normalised) return status;
    }
    return OrderStatus.placed;
  }

  /// Serialises to the camelCase [name] expected by the API.
  String toJson() => name;

  /// Whether the order has reached a final state.
  bool get isTerminal =>
      this == OrderStatus.completed ||
      this == OrderStatus.cancelled ||
      this == OrderStatus.rejected;

  /// Whether the order ended unsuccessfully.
  bool get isNegative =>
      this == OrderStatus.cancelled || this == OrderStatus.rejected;
}
