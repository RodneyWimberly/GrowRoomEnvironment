import { Injectable } from '@angular/core';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import * as generated from './endpoint.services';

export type RolesChangedOperation = 'add' | 'delete' | 'modify';
export interface RolesChangedEventArg { roles: generated.RoleViewModel[] | string[]; operation: RolesChangedOperation; }

@Injectable()
export class AccountService {
    public static readonly roleAddedOperation: RolesChangedOperation = 'add';
    public static readonly roleDeletedOperation: RolesChangedOperation = 'delete';
    public static readonly roleModifiedOperation: RolesChangedOperation = 'modify';
    private _rolesChanged = new Subject<RolesChangedEventArg>();

    constructor(private authEndpointService: generated.AuthEndpointService, private accountEndpointService: generated.AccountEndpointService) {
    }
    getUser(userId?: string) {
        return userId ? this.accountEndpointService.getUserById(userId) : this.accountEndpointService.getCurrentUser();
    }
    getUserAndRoles(userId?: string) {
        return forkJoin(this.getUser(userId), this.accountEndpointService.getRolesAll());
    }
    getUsers(page?: number, pageSize?: number) {
        return page && pageSize ? this.accountEndpointService.getUsers(page, pageSize) : this.accountEndpointService.getUsersAll();
    }
    getUsersAndRoles(page?: number, pageSize?: number) {
        return forkJoin(this.getUsers(page, pageSize), this.accountEndpointService.getRolesAll());
    }
    updateUser(user: generated.UserEditViewModel) {
        if (user.id) {
            return this.accountEndpointService.updateUser(user.id, user);
        }
        else {
            return this.accountEndpointService.getUserByUserName(user.userName).pipe(mergeMap(foundUser => {
                user.id = foundUser.id;
                return this.accountEndpointService.updateUser(user.id, user);
            }));
        }
    }
    newUser(user: generated.UserEditViewModel) {
        return this.accountEndpointService.register(user);
    }
    getUserPreferences() {
        return this.accountEndpointService.userPreferences();
    }
    updateUserPreferences(configuration: string) {
        return this.accountEndpointService.userPreferences2(configuration);
    }
    deleteUser(userOrUserId: string | generated.UserViewModel): Observable<generated.UserViewModel> {
        if (typeof userOrUserId === 'string' || userOrUserId instanceof String) {
            return this.accountEndpointService.deleteUser(userOrUserId as string).pipe<generated.UserViewModel>(tap(data => this.onRolesUserCountChanged(data.roles)));
        }
        else {
            if (userOrUserId.id) {
                return this.deleteUser(userOrUserId.id);
            }
            else {
                return this.accountEndpointService.getUserByUserName(userOrUserId.userName).pipe<generated.UserViewModel>(tap(user => this.deleteUser(user.id)));
            }
        }
    }
    unblockUser(userId: string) {
        return this.accountEndpointService.unblockUser(userId);
    }
    userHasPermission(permissionValue: generated.PermissionValue): boolean {
        return this.permissions.some(p => p == permissionValue);
    }
    refreshLoggedInUser() {
        return this.accountEndpointService.refreshLogin();
    }
    getRoles(page?: number, pageSize?: number) {
        return page && pageSize ? this.accountEndpointService.getRoles(page, pageSize) : this.accountEndpointService.getRolesAll();
    }
    getRolesAndPermissions(page?: number, pageSize?: number) {
        return forkJoin(this.getRoles(page, pageSize), this.accountEndpointService.getAllPermissions());
    }
    updateRole(role: generated.RoleViewModel) {
        if (role.id) {
            return this.accountEndpointService.updateRole(role.id, role).pipe(tap(data => this.onRolesChanged([role], AccountService.roleModifiedOperation)));
        }
        else {
            return this.accountEndpointService.getRoleByName(role.name).pipe(mergeMap(foundRole => {
                role.id = foundRole.id;
                return this.accountEndpointService.updateRole(role.id, role);
            }), tap(data => this.onRolesChanged([role], AccountService.roleModifiedOperation)));
        }
    }
    newRole(role: generated.RoleViewModel) {
        return this.accountEndpointService.createRole(role).pipe<generated.RoleViewModel>(tap(data => this.onRolesChanged([role], AccountService.roleAddedOperation)));
    }
    deleteRole(roleOrRoleId: string | generated.RoleViewModel): Observable<generated.RoleViewModel> {
        if (typeof roleOrRoleId === 'string' || roleOrRoleId instanceof String) {
            return this.accountEndpointService.deleteRole(roleOrRoleId as string).pipe<generated.RoleViewModel>(tap(data => this.onRolesChanged([data], AccountService.roleDeletedOperation)));
        }
        else {
            if (roleOrRoleId.id) {
                return this.deleteRole(roleOrRoleId.id);
            }
            else {
                return this.accountEndpointService.getRoleByName(roleOrRoleId.name).pipe<generated.RoleViewModel>(tap(role => this.deleteRole(role.id)));
            }
        }
    }
    getPermissions() {
        return this.accountEndpointService.getAllPermissions();
    }
    private onRolesChanged(roles: generated.RoleViewModel[] | string[], op: RolesChangedOperation) {
        this._rolesChanged.next({ roles, operation: op });
    }
    onRolesUserCountChanged(roles: generated.RoleViewModel[] | string[]) {
        return this.onRolesChanged(roles, AccountService.roleModifiedOperation);
    }
    getRolesChangedEvent(): Observable<RolesChangedEventArg> {
        return this._rolesChanged.asObservable();
    }
    get permissions(): generated.PermissionValue[] {
        return this.authEndpointService.userPermissions;
    }
    get currentUser() {
        return this.authEndpointService.currentUser;
    }
}
