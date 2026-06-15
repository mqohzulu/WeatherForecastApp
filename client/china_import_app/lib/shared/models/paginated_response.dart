/// Generic paginated envelope returned by list endpoints.
///
/// Matches the .NET API shape:
/// ```json
/// { "items": [...], "page": 1, "pageSize": 20, "totalCount": 134, "totalPages": 7 }
/// ```
class PaginatedResponse<T> {
  const PaginatedResponse({
    required this.items,
    required this.page,
    required this.pageSize,
    required this.totalCount,
    required this.totalPages,
  });

  final List<T> items;
  final int page;
  final int pageSize;
  final int totalCount;
  final int totalPages;

  bool get hasNextPage => page < totalPages;
  bool get isEmpty => items.isEmpty;

  factory PaginatedResponse.fromJson(
    Map<String, dynamic> json,
    T Function(Map<String, dynamic> json) fromJsonT,
  ) {
    final rawItems = (json['items'] as List<dynamic>? ?? <dynamic>[]);
    return PaginatedResponse<T>(
      items: rawItems
          .map((e) => fromJsonT(e as Map<String, dynamic>))
          .toList(growable: false),
      page: json['page'] as int? ?? 1,
      pageSize: json['pageSize'] as int? ?? rawItems.length,
      totalCount: json['totalCount'] as int? ?? rawItems.length,
      totalPages: json['totalPages'] as int? ?? 1,
    );
  }

  PaginatedResponse<R> map<R>(R Function(T item) transform) {
    return PaginatedResponse<R>(
      items: items.map(transform).toList(growable: false),
      page: page,
      pageSize: pageSize,
      totalCount: totalCount,
      totalPages: totalPages,
    );
  }
}
