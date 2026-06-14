import 'package:json_annotation/json_annotation.dart';

import '../../domain/entities/category.dart';

part 'category_model.g.dart';

/// Data model for a category, matching the .NET `CategoryDto` (camelCase).
@JsonSerializable()
class CategoryModel {
  const CategoryModel({
    required this.id,
    required this.name,
    this.imageUrl,
    this.productCount,
  });

  final String id;
  final String name;
  final String? imageUrl;
  final int? productCount;

  factory CategoryModel.fromJson(Map<String, dynamic> json) =>
      _$CategoryModelFromJson(json);

  Map<String, dynamic> toJson() => _$CategoryModelToJson(this);

  Category toEntity() => Category(
        id: id,
        name: name,
        imageUrl: imageUrl,
        productCount: productCount,
      );
}
