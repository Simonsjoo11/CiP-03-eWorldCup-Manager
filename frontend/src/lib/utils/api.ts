export function extractMaxRounds(rawMax: unknown): number | undefined {
  if (typeof rawMax === 'number' && Number.isFinite(rawMax)) {
    return rawMax;
  }

  if (
    rawMax &&
    typeof rawMax === 'object' &&
    'max' in rawMax &&
    typeof (rawMax as { max: unknown }).max === 'number' &&
    Number.isFinite((rawMax as { max: number }).max)
  ) {
    return (rawMax as { max: number }).max;
  }

  return undefined;
}

export function hasMaxProperty(value: unknown): value is { max: number } {
  return (
    value !== null &&
    typeof value === 'object' &&
    'max' in value &&
    typeof (value as { max: unknown }).max === 'number' &&
    Number.isFinite((value as { max: unknown }).max)
  );
}
