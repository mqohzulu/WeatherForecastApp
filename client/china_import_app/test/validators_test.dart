import 'package:china_import_app/core/utils/validators.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  group('Validators.saMobile', () {
    test('accepts valid local-format numbers', () {
      expect(Validators.saMobile('0821234567'), isNull);
      expect(Validators.saMobile('082 123 4567'), isNull);
      expect(Validators.saMobile('0731234567'), isNull);
      expect(Validators.saMobile('0601234567'), isNull);
    });

    test('accepts valid +27 international numbers', () {
      expect(Validators.saMobile('+27821234567'), isNull);
      expect(Validators.saMobile('+27 82 123 4567'), isNull);
    });

    test('rejects empty / null input', () {
      expect(Validators.saMobile(null), isNotNull);
      expect(Validators.saMobile(''), isNotNull);
      expect(Validators.saMobile('   '), isNotNull);
    });

    test('rejects malformed numbers', () {
      expect(Validators.saMobile('12345'), isNotNull);
      expect(Validators.saMobile('0123456789'), isNotNull); // 01x not mobile
      expect(Validators.saMobile('+1234567890'), isNotNull);
      expect(Validators.saMobile('082123456'), isNotNull); // too short
    });
  });

  group('Validators.normaliseSaMobile', () {
    test('converts local format to E.164', () {
      expect(Validators.normaliseSaMobile('0821234567'), '+27821234567');
      expect(Validators.normaliseSaMobile('082 123 4567'), '+27821234567');
    });

    test('leaves +27 numbers unchanged (minus spaces)', () {
      expect(Validators.normaliseSaMobile('+27 82 123 4567'), '+27821234567');
    });

    test('prefixes a bare 27 number with +', () {
      expect(Validators.normaliseSaMobile('27821234567'), '+27821234567');
    });
  });

  group('Validators.otp', () {
    test('accepts a correct-length numeric code', () {
      expect(Validators.otp('123456'), isNull);
    });

    test('rejects wrong length or non-numeric codes', () {
      expect(Validators.otp('12345'), isNotNull);
      expect(Validators.otp('1234567'), isNotNull);
      expect(Validators.otp('12a456'), isNotNull);
      expect(Validators.otp(null), isNotNull);
    });

    test('respects a custom length', () {
      expect(Validators.otp('1234', length: 4), isNull);
      expect(Validators.otp('123456', length: 4), isNotNull);
    });
  });

  group('Validators.positiveInt', () {
    test('accepts positive integers', () {
      expect(Validators.positiveInt('1'), isNull);
      expect(Validators.positiveInt('42'), isNull);
    });

    test('rejects zero, negatives and non-numbers', () {
      expect(Validators.positiveInt('0'), isNotNull);
      expect(Validators.positiveInt('-3'), isNotNull);
      expect(Validators.positiveInt('abc'), isNotNull);
      expect(Validators.positiveInt(null), isNotNull);
    });
  });
}
