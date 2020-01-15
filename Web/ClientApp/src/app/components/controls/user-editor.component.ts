import { Component, OnInit, ViewChild, Input } from '@angular/core';

import { AlertService, MessageSeverity } from '../../services/alert.service';
import { AccountService } from "../../services/account.service";
import { Utilities } from '../../helpers/utilities';
import * as generated from '../../services/endpoint.services';

@Component({
    selector: 'user-editor',
    templateUrl: './user-editor.component.html',
    styleUrls: ['./user-editor.component.scss']
})
export class UserEditorComponent implements OnInit {

    public isEditMode = false;
    public isNewUser = false;
    public isSaving = false;
    public isChangePassword = false;
    public isEditingSelf = false;
    public showValidationErrors = false;
    public uniqueId: string = Utilities.uniqueId();
    public user: generated.UserViewModel = new generated.UserViewModel();
    public userEdit: generated.UserEditViewModel;
    public allRoles: generated.RoleViewModel[] = [];

    public formResetToggle = true;

    public changesSavedCallback: () => void;
    public changesFailedCallback: () => void;
    public changesCancelledCallback: () => void;

    @Input()
    isViewOnly: boolean;

    @Input()
    isGeneralEditor = false;





    @ViewChild('f', { static: false })
    public form;

    // ViewChilds Required because ngIf hides template variables from global scope
    @ViewChild('userName', { static: false })
    public userName;

    @ViewChild('userPassword', { static: false })
    public userPassword;

    @ViewChild('email', { static: false })
    public email;

    @ViewChild('currentPassword', { static: false })
    public currentPassword;

    @ViewChild('newPassword', { static: false })
    public newPassword;

    @ViewChild('confirmPassword', { static: false })
    public confirmPassword;

    @ViewChild('roles', { static: false })
    public roles;

    @ViewChild('rolesSelector', { static: false })
    public rolesSelector;


    constructor(private alertService: AlertService, private accountClient: AccountService) {
    }

    ngOnInit() {
        if (!this.isGeneralEditor) {
            this.loadCurrentUserData();
        }
    }



    private loadCurrentUserData() {
        this.alertService.startLoadingMessage();

        if (this.canViewAllRoles) {
            this.accountClient.getUserAndRoles().subscribe(results => this.onCurrentUserDataLoadSuccessful(results[0], results[1]), error => this.onCurrentUserDataLoadFailed(error));
        } else {
            this.accountClient.getUser().subscribe(user => this.onCurrentUserDataLoadSuccessful(user, user.roles.map(x => { let r = new generated.RoleViewModel(); r.name = x; return r; })), error => this.onCurrentUserDataLoadFailed(error));
        }
    }


    private onCurrentUserDataLoadSuccessful(user: generated.UserViewModel, roles: generated.RoleViewModel[]) {
        this.alertService.stopLoadingMessage();
        this.user = user;
        this.allRoles = roles;
    }

    private onCurrentUserDataLoadFailed(error: any) {
        this.alertService.stopLoadingMessage();
        this.alertService.showStickyMessage('Load Error', `Unable to retrieve user data from the server.\r\nErrors: "${Utilities.getHttpResponseMessages(error)}"`,
            MessageSeverity.error, error);

        this.user = new generated.UserViewModel();
    }



    getRoleByName(name: string) {
        return this.allRoles.find((r) => r.name == name);
    }



    showErrorAlert(caption: string, message: string) {
        this.alertService.showMessage(caption, message, MessageSeverity.error);
    }


    deletePasswordFromUser(user: generated.UserEditViewModel | generated.UserViewModel) {
        const userEdit = user as generated.UserEditViewModel;

        delete userEdit.currentPassword;
        delete userEdit.newPassword;
    }


    edit() {
        if (!this.isGeneralEditor) {
            this.isEditingSelf = true;
            this.userEdit = new generated.UserEditViewModel();
            Object.assign(this.userEdit, this.user);
        } else {
            if (!this.userEdit) {
                this.userEdit = new generated.UserEditViewModel();
            }

            this.isEditingSelf = this.accountClient.currentUser ? this.userEdit.id == this.accountClient.currentUser.id : false;
        }

        this.isEditMode = true;
        this.showValidationErrors = true;
        this.isChangePassword = false;
    }


    save() {
        this.isSaving = true;
        this.alertService.startLoadingMessage('Saving changes...');

        if (this.isNewUser) {
            this.accountClient.newUser(this.userEdit).subscribe(user => this.saveSuccessHelper(user), error => this.saveFailedHelper(error));
        } else {
            this.accountClient.updateUser(this.userEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
        }
    }


    private saveSuccessHelper(user?: generated.UserViewModel) {
        this.testIsRoleUserCountChanged(this.user, this.userEdit);

        if (user) {
            Object.assign(this.userEdit, user);
        }

        this.isSaving = false;
        this.alertService.stopLoadingMessage();
        this.isChangePassword = false;
        this.showValidationErrors = false;

        this.deletePasswordFromUser(this.userEdit);
        Object.assign(this.user, this.userEdit);
        this.userEdit = new generated.UserEditViewModel();
        this.resetForm();


        if (this.isGeneralEditor) {
            if (this.isNewUser) {
                this.alertService.showMessage('Success', `User \"${this.user.userName}\" was created successfully`, MessageSeverity.success);
            } else if (!this.isEditingSelf) {
                this.alertService.showMessage('Success', `Changes to user \"${this.user.userName}\" was saved successfully`, MessageSeverity.success);
            }
        }

        if (this.isEditingSelf) {
            this.alertService.showMessage('Success', 'Changes to your User Profile was saved successfully', MessageSeverity.success);
            this.refreshLoggedInUser();
        }

        this.isEditMode = false;


        if (this.changesSavedCallback) {
            this.changesSavedCallback();
        }
    }


    private saveFailedHelper(error: any) {
        this.isSaving = false;
        this.alertService.stopLoadingMessage();
        this.alertService.showStickyMessage('Save Error', 'The below errors occured whilst saving your changes:', MessageSeverity.error, error);
        this.alertService.showStickyMessage(error, null, MessageSeverity.error);

        if (this.changesFailedCallback) {
            this.changesFailedCallback();
        }
    }



    private testIsRoleUserCountChanged(currentUser: generated.UserViewModel, editedUser: generated.UserViewModel) {

        const rolesAdded = this.isNewUser ? editedUser.roles : editedUser.roles.filter(role => currentUser.roles.indexOf(role) == -1);
        const rolesRemoved = this.isNewUser ? [] : currentUser.roles.filter(role => editedUser.roles.indexOf(role) == -1);

        const modifiedRoles = rolesAdded.concat(rolesRemoved);

        if (modifiedRoles.length) {
            setTimeout(() => this.accountClient.onRolesUserCountChanged(modifiedRoles));
        }
    }



    cancel() {
        if (this.isGeneralEditor) {
            this.userEdit = this.user = new generated.UserEditViewModel();
        } else {
            this.userEdit = new generated.UserEditViewModel();
        }

        this.showValidationErrors = false;
        this.resetForm();

        this.alertService.showMessage('Cancelled', 'Operation cancelled by user', MessageSeverity.default);
        this.alertService.resetStickyMessage();

        if (!this.isGeneralEditor) {
            this.isEditMode = false;
        }

        if (this.changesCancelledCallback) {
            this.changesCancelledCallback();
        }
    }


    close() {
        this.userEdit = this.user = new generated.UserEditViewModel();
        this.showValidationErrors = false;
        this.resetForm();
        this.isEditMode = false;

        if (this.changesSavedCallback) {
            this.changesSavedCallback();
        }
    }



    private refreshLoggedInUser() {
        this.accountClient.refreshLoggedInUser()
            .subscribe(user => {
                this.loadCurrentUserData();
            },
                error => {
                    this.alertService.resetStickyMessage();
                    this.alertService.showStickyMessage('Refresh failed', 'An error occured whilst refreshing logged in user information from the server', MessageSeverity.error, error);
                });
    }


    changePassword() {
        this.isChangePassword = true;
    }


    unlockUser() {
        this.isSaving = true;
        this.alertService.startLoadingMessage('Unblocking user...');


        this.accountClient.unblockUser(this.userEdit.id)
            .subscribe(() => {
                this.isSaving = false;
                //this.userEdit.isLockedOut = false;
                this.alertService.stopLoadingMessage();
                this.alertService.showMessage('Success', 'User has been successfully unblocked', MessageSeverity.success);
            },
                error => {
                    this.isSaving = false;
                    this.alertService.stopLoadingMessage();
                    this.alertService.showStickyMessage('Unblock Error', 'The below errors occured whilst unblocking the user:', MessageSeverity.error, error);
                    this.alertService.showStickyMessage(error, null, MessageSeverity.error);
                });
    }


    resetForm(replace = false) {
        this.isChangePassword = false;

        if (!replace) {
            this.form.reset();
        } else {
            this.formResetToggle = false;

            setTimeout(() => {
                this.formResetToggle = true;
            });
        }
    }


    newUser(allRoles: generated.RoleViewModel[]) {
        this.isGeneralEditor = true;
        this.isNewUser = true;

        this.allRoles = [...allRoles];
        this.user = this.userEdit = new generated.UserEditViewModel();
        this.userEdit.isEnabled = true;
        this.edit();

        return this.userEdit;
    }

    editUser(user: generated.UserViewModel, allRoles: generated.RoleViewModel[]) {
        if (user) {
            this.isGeneralEditor = true;
            this.isNewUser = false;

            this.setRoles(user, allRoles);
            this.user = new generated.UserViewModel();
            this.userEdit = new generated.UserEditViewModel();
            Object.assign(this.user, user);
            Object.assign(this.userEdit, user);
            this.edit();

            return this.userEdit;
        } else {
            return this.newUser(allRoles);
        }
    }


    displayUser(user: generated.UserViewModel, allRoles?: generated.RoleViewModel[]) {

        this.user = new generated.UserViewModel();
        Object.assign(this.user, user);
        this.deletePasswordFromUser(this.user);
        this.setRoles(user, allRoles);

        this.isEditMode = false;
    }



    private setRoles(user: generated.UserViewModel, allRoles?: generated.RoleViewModel[]) {

        this.allRoles = allRoles ? [...allRoles] : [];

        if (user.roles) {
            for (let ur of user.roles) {
                if (!this.allRoles.some(r => r.name == ur)) {
                    let rl = new generated.RoleViewModel();
                    rl.name = ur;
                    this.allRoles.unshift(rl);
                }

            }
        }

        if (allRoles == null || this.allRoles.length != allRoles.length) {
            setTimeout(() => {
                if (this.rolesSelector) {
                    this.rolesSelector.refresh();
                }
            });
        }
    }



    get canViewAllRoles() {
        return this.accountClient.userHasPermission(generated.PermissionValues.ViewRoles);
    }

    get canAssignRoles() {
        return this.accountClient.userHasPermission(generated.PermissionValues.AssignRoles);
    }
}
