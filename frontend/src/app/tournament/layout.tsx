import { ReactNode } from 'react';
import ErrorBoundary from '@/components/ui/ErrorBoundary';

export default function TournamentLayout({ children }: { children: ReactNode }) {
  return <ErrorBoundary>{children}</ErrorBoundary>;
}
