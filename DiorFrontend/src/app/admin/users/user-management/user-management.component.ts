import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserService } from '../../../services/user.service';
import { RoleService } from '../role.service';
import { PrivilegeService } from '../privilege.service';
import { AccessService } from '../../../services/access.service';
import { AccessCompetencyService } from '../access-competency.service';
import { UserDto } from '../../../models/user.model';
import { RoleDefinition } from '../../../models/role-definition.model';
import { PrivilegeDto } from '../../../models/privilege.model';
import { UserAccess } from '../../../models/user-access.model';
import { AccessCompetency } from '../../../models/access-competency.model';
import { AccessDto } from '../../../models/access.model';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss'],
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule]
})
export class UserManagementComponent implements OnInit {
  users: UserDto[] = [];
  roles: RoleDefinition[] = [];
  privileges: PrivilegeDto[] = [];
  accesses: UserAccess[] = [];
  competencies: AccessCompetency[] = [];
  selectedUser: UserDto | null = null;

  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private roleService: RoleService,
    private privilegeService: PrivilegeService,
    private accessService: AccessService,
    private accessCompetencyService: AccessCompetencyService
  ) {
    this.form = this.fb.group({
      id: [],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: [''],
      isActive: [true],
      phoneNumber: [''],
      badgePhysicalNumber: [''],
      roles: [[], Validators.required],
      accessIds: [[]],
      accessCompetencyIds: [[]]
    });
  }

  ngOnInit() {
    this.loadAll();
    this.form.get('roles')?.valueChanges.subscribe(roleIds => {
      if (roleIds && roleIds.length) {
        this.privilegeService.getByRoleIds(roleIds).subscribe((p: PrivilegeDto[]) => this.privileges = p);
      } else {
        this.privileges = [];
      }
    });
  }

  loadAll() {
    this.userService.getAll().subscribe((u: UserDto[]) => this.users = u);
    this.roleService.getAll().subscribe((r: RoleDefinition[]) => this.roles = r);
   this.accessService.getAll().subscribe((a: AccessDto[]) => {
});
    this.accessCompetencyService.getAll().subscribe((c: AccessCompetency[]) => this.competencies = c);
  }

  selectUser(user: UserDto) {
    this.selectedUser = user;
    this.userService.getById(user.id!).subscribe(u => {
      this.form.patchValue(u);
      this.form.get('password')?.reset();
    });
    // Les relations secondaires doivent être chargées via les services dédiés
    // Les propriétés directes n'existent plus sur UserDto
    // this.form.get('roles')?.setValue(user.roles);
    // this.form.get('accessIds')?.setValue(user.accessIds);
    // this.form.get('accessCompetencyIds')?.setValue(user.accessCompetencyIds);
  }

  newUser() {
    this.selectedUser = null;
    this.form.reset({ isActive: true });
    this.privileges = [];
  }

  save() {
    const user: UserDto = this.form.value;
    if (user.id) {
      this.userService.update(user.id, user).subscribe(() => this.loadAll());
    } else {
      this.userService.create(user).subscribe(() => this.loadAll());
    }
    this.newUser();
  }

  delete(user: UserDto) {
    if (user.id && confirm('Supprimer cet utilisateur ?')) {
      this.userService.delete(user.id).subscribe(() => this.loadAll());
      this.newUser();
    }
  }
}
