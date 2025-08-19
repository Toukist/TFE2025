import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { UserService } from './user.service';
import { environment } from '../../environments/environment';

describe('UserService', () => {
  let service: UserService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [UserService]
    });
    service = TestBed.inject(UserService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    http.verify();
  });

  it('getAll() doit appeler endpoint /User', () => {
    service.getAll().subscribe();
    const req = http.expectOne(`${environment.apiUrl}/User`);
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('create() doit POST vers /User', () => {
    service.create({ userName:'u', firstName:'f', lastName:'l', email:'e@mail', isActive:true } as any).subscribe();
    const req = http.expectOne(`${environment.apiUrl}/User`);
    expect(req.request.method).toBe('POST');
    req.flush({ id:1 });
  });
});
