import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'joinNames', standalone: true })
export class JoinNamesPipe implements PipeTransform {
  transform(arr: { name: string }[] | undefined): string {
    return arr?.map(x => x.name).join(', ') || '';
  }
}
