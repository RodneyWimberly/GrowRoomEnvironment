import { Injectable } from '@angular/core';
import { OAuthStorage } from 'angular-oauth2-oidc';
import { LocalStorageService } from '../services/local-storage.service';

@Injectable()
export class AuthStorageService implements OAuthStorage {

  constructor(private localStorage: LocalStorageService) {
  }

  static RememberMe = false;
  private dbKeyPrefix = 'AUTH:';

  getItem(key: string): string {
    return this.localStorage.getData(this.dbKeyPrefix + key);
  }

  removeItem(key: string): void {
    this.localStorage.deleteData(this.dbKeyPrefix + key);
  }

  setItem(key: string, data: string): void {
    if (AuthStorageService.RememberMe) {
      this.localStorage.savePermanentData(data, this.dbKeyPrefix + key);
    } else {
      this.localStorage.saveSyncedSessionData(data, this.dbKeyPrefix + key);
    }
  }
}
