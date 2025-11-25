export const formatOrdinal = (num: number): string => {
  const suffixes = ['th', 'st', 'nd', 'rd'];
  const value = num % 100;
  return num + (suffixes[(value - 20) % 10] || suffixes[value] || suffixes[0]);
};

export const formatPlayerCount = (count: number): string => {
  return `${count} player${count !== 1 ? 's' : ''}`;
};

export const formatRoundProgress = (current: number, total: number): string => {
  return `Round ${current} of ${total}`;
};
