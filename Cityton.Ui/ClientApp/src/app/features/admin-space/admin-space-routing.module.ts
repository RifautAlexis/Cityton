import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { IODataComponent } from './pages/IO-data/IO-data.component';
import { UserManagementComponent } from './pages/user-management/user-management.component';
import { GroupsComponent } from './pages/groups/groups.component';

const routes: Routes = [
    { path: '', component: UserManagementComponent },
    { path: 'groups', component: GroupsComponent },
    { path: 'groups/toSearch', component: GroupsComponent },
    { path: 'data', component: IODataComponent },
    { path: 'user', component: UserManagementComponent }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class AdminSpaceRoutingModule { }
