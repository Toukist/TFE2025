import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { AdminUsersComponent } from './admin-users.component';
import { UserService } from '../../services/user.service';
import { environment } from '../../../environments/environment';
import { of } from 'rxjs';

const mockUsers = [
  { id: 1, userName: 'jdoe', firstName: 'John', lastName: 'Doe', email: 'j@d.io', isActive: true },
  { id: 2, userName: 'asmith', firstName: 'Alice', lastName: 'Smith', email: 'a@d.io', isActive: false }
];

describe('AdminUsersComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminUsersComponent, HttpClientTestingModule],
      providers: [provideNoopAnimations(), UserService]
    }).compileComponents();
  });

  it('charge et affiche les utilisateurs', () => {
    const fixture = TestBed.createComponent(AdminUsersComponent);
    const comp = fixture.componentInstance;
    const http = TestBed.inject(HttpTestingController);
    fixture.detectChanges();

    const req = http.expectOne(`${environment.apiUrl}/User`);
    req.flush(mockUsers);

    fixture.detectChanges();
    expect(comp.dataSource.data.length).toBe(2);
  });
});
