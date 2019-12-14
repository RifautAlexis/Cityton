import { environment } from './../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { IUser as User } from '@shared/models/User';
import { IUserToUpdate as UserToUpdate } from '@shared/models/UserToUpdate';

import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http: HttpClient) {
  }

  update(userToUpdate: UserToUpdate): Observable<User> {
    return this.http.put<User>(environment.apiUrl + 'user/update/' + userToUpdate.id, userToUpdate);
  }

  searchUser(username: string, securityLevel: string): Observable<User[]> {

    let params = new HttpParams()
      .set('sl', securityLevel) //sl => securityLevel
      .set('q', username); //q => query

    return this.http.get<User[]>(environment.apiUrl + 'user/searchUser', { params });
  }

  uploadPicture(file: File, userId: number): Observable<string> {

    const formData = new FormData();
    formData.append('file', file);

    return this.http.put<string>(environment.apiUrl + 'user/uploadPicture/' + userId, formData);
  }

  isUniqueEmail(email: string) {
    return this.http.get<boolean>(environment.apiUrl + 'user/isUniqueEmail/' + email);
  }

  isUniquePhoneNumber(phoneNumber: string) {
    return this.http.get<boolean>(environment.apiUrl + 'user/isUniquePhoneNumber/' + phoneNumber);
  }

  isUniqueUsername(username: string) {
    return this.http.get<boolean>(environment.apiUrl + 'user/isUniqueUsername/' + username);
  }

}
