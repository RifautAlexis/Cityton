import { Component, OnInit } from '@angular/core';
import { Validators, FormBuilder, FormGroup, NgForm } from '@angular/forms';
import { Router } from '@angular/router';

import { UserService } from '@core/services/user.service';
import { AuthService } from '@core/services/auth.service';

import { IUser as User } from '@shared/models/User';
import { IUserToUpdate as UserToUpdate } from '@shared/models/UserToUpdate';

@Component({
  selector: 'app-change-email',
  templateUrl: './change-email.component.html'
})

export class ChangeEmailComponent implements OnInit {

  emailForm: FormGroup;

  constructor(private formBuilder: FormBuilder, private userService: UserService, private authService: AuthService) { }

  ngOnInit() {
    this.emailForm = this.formBuilder.group({
      email: ['', Validators.required]
    });
  }

  onSubmit() {
    if (this.emailForm.invalid) {
      return;
    }

    let currentUser: User = this.authService.currentUserValue();

    let user: UserToUpdate = {
      id: currentUser.id,
      username: currentUser.username,
      phoneNumber: currentUser.phoneNumber,
      email: this.emailForm.controls.email.value,
      picture: currentUser.picture,
      role: currentUser.role,
      token: this.authService.currentTokenValue(),
      password: ""
    };

    this.userService.update(user).subscribe(
      (user: User) => {
        this.authService.updateCurrentUser(user);
        // this.router.navigate(['chat']);
      },
      (error: any) => {
        console.log(error);
      }
    );
  }

  cancel(formToReset: NgForm) {
    formToReset.reset();
  }

}
