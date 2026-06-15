import 'package:json_annotation/json_annotation.dart';

import '../../domain/entities/announcement.dart';

part 'announcement_model.g.dart';

/// Wire model for `GET /announcements` items.
@JsonSerializable()
class AnnouncementModel {
  const AnnouncementModel({
    required this.id,
    required this.title,
    required this.body,
    required this.publishedAt,
    this.imageUrl,
    this.isPinned = false,
  });

  final String id;
  final String title;
  final String body;
  final DateTime publishedAt;
  final String? imageUrl;
  final bool isPinned;

  factory AnnouncementModel.fromJson(Map<String, dynamic> json) =>
      _$AnnouncementModelFromJson(json);

  Map<String, dynamic> toJson() => _$AnnouncementModelToJson(this);

  Announcement toEntity() => Announcement(
        id: id,
        title: title,
        body: body,
        publishedAt: publishedAt,
        imageUrl: imageUrl,
        isPinned: isPinned,
      );
}
