import { Component, ViewChild } from '@angular/core';

import { AlertService, MessageSeverity } from '../../services/alert.service';
import { AccountService } from "../../services/account.service";
import * as generated from '../../services/endpoint.services';


@Component({
    selector: 'role-editor',
    templateUrl: './role-editor.component.html',
    styleUrls: ['./role-editor.component.scss']
})
export class RoleEditorComponent {

    private isNewRole = false;
    public isSaving: boolean;
    public showValidationErrors = true;
    public roleEdit: generated.RoleViewModel = new generated.RoleViewModel();
    public allPermissions: generated.PermissionViewModel[] = [];
    public selectedValues: { [key: string]: boolean; } = {};
    private editingRoleName: string;

    public formResetToggle = true;

    public changesSavedCallback: () => void;
    public changesFailedCallback: () => void;
    public changesCancelledCallback: () => void;


    @ViewChild('f', { static: false })
    private form;



    constructor(private alertService: AlertService, private accountClient: AccountService) {
    }



    showErrorAlert(caption: string, message: string) {
        this.alertService.showMessage(caption, message, MessageSeverity.error);
    }


    save() {
        this.isSaving = true;
        this.alertService.startLoadingMessage('Saving changes...');

        this.roleEdit.permissions = this.getSelectedPermissions();

        if (this.isNewRole) {
            this.accountClient.newRole(this.roleEdit).subscribe(role => this.saveSuccessHelper(role), error => this.saveFailedHelper(error));
        } else {
            this.accountClient.updateRole(this.roleEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
        }
    }




    private saveSuccessHelper(role?: generated.RoleViewModel) {
        if (role) {
            Object.assign(this.roleEdit, role);
        }

        this.isSaving = false;
        this.alertService.stopLoadingMessage();
        this.showValidationErrors = false;

        if (this.isNewRole) {
            this.alertService.showMessage('Success', `Role \"${this.roleEdit.name}\" was created successfully`, MessageSeverity.success);
        } else {
            this.alertService.showMessage('Success', `Changes to role \"${this.roleEdit.name}\" was saved successfully`, MessageSeverity.success);
        }


        this.roleEdit = new generated.RoleViewModel();
        this.resetForm();


        if (!this.isNewRole && this.accountClient.currentUser.roles.some(r => r == this.editingRoleName)) {
            this.refreshLoggedInUser();
        }

        if (this.changesSavedCallback) {
            this.changesSavedCallback();
        }
    }


    private refreshLoggedInUser() {
        this.accountClient.refreshLoggedInUser()
            .subscribe(user => { },
                error => {
                    this.alertService.resetStickyMessage();
                    this.alertService.showStickyMessage('Refresh failed', 'An error occured whilst refreshing logged in user information from the server', MessageSeverity.error, error);
                });
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


    cancel() {
        this.roleEdit = new generated.RoleViewModel();

        this.showValidationErrors = false;
        this.resetForm();

        this.alertService.showMessage('Cancelled', 'Operation cancelled by user', MessageSeverity.default);
        this.alertService.resetStickyMessage();

        if (this.changesCancelledCallback) {
            this.changesCancelledCallback();
        }
    }



    selectAll() {
        this.allPermissions.forEach(p => this.selectedValues[p.value] = true);
    }


    selectNone() {
        this.allPermissions.forEach(p => this.selectedValues[p.value] = false);
    }


    toggleGroup(groupName: string) {
        let firstMemberValue: boolean;

        this.allPermissions.forEach(p => {
            if (p.groupName != groupName) {
                return;
            }

            if (firstMemberValue == null) {
                firstMemberValue = this.selectedValues[p.value] == true;
            }

            this.selectedValues[p.value] = !firstMemberValue;
        });
    }


    private getSelectedPermissions() {
        return this.allPermissions.filter(p => this.selectedValues[p.value] == true);
    }


    resetForm(replace = false) {

        if (!replace) {
            this.form.reset();
        } else {
            this.formResetToggle = false;

            setTimeout(() => {
                this.formResetToggle = true;
            });
        }
    }


    newRole(allPermissions: generated.PermissionViewModel[]) {
        this.isNewRole = true;
        this.showValidationErrors = true;

        this.editingRoleName = null;
        this.allPermissions = allPermissions;
        this.selectedValues = {};
        this.roleEdit = new generated.RoleViewModel();

        return this.roleEdit;
    }

    editRole(role: generated.RoleViewModel, allPermissions: generated.PermissionViewModel[]) {
        if (role) {
            this.isNewRole = false;
            this.showValidationErrors = true;

            this.editingRoleName = role.name;
            this.allPermissions = allPermissions;
            this.selectedValues = {};
            role.permissions.forEach(p => this.selectedValues[p.value] = true);
            this.roleEdit = new generated.RoleViewModel();
            Object.assign(this.roleEdit, role);

            return this.roleEdit;
        } else {
            return this.newRole(allPermissions);
        }
    }



    get canManageRoles() {
        return this.accountClient.userHasPermission(generated.PermissionValues.ManageRoles);
    }
}
