import * as generated from '../services/endpoint.services';


export interface AccessTokenModel {
  nbf: number;
  exp: number;
  iss: string;
  aud: string | string[];
  client_id: string;
  sub: string;
  auth_time: number;
  idp: string;
  role: string | string[];
  permission: generated.PermissionValue | generated.PermissionValue[];
  name: string;
  email: string;
  phone_number: string;
  fullname: string;
  jobtitle: string;
  configuration: string;
  scope: string | string[];
  amr: string[];
}
