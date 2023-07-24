import { useRef } from 'react';
import { Flippo } from '../dist';

export function useAppFlippo() {
  const ref = useRef(new Flippo());
  return ref.current;
}
