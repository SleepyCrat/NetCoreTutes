import { Injectable } from '@angular/core';
import { AngularFireAuth } from '@angular/fire/auth'
import * as firebase from 'firebase/app';
import { BehaviorSubject, Observable } from 'rxjs';
import { CurrentUser } from '../models/current-user';

@Injectable({
  providedIn: 'root'
})
export class AuthServiceService {

  user$: BehaviorSubject<CurrentUser> = new BehaviorSubject<CurrentUser>(new CurrentUser());

  constructor(private angularAuth: AngularFireAuth) {
    this.angularAuth.authState.subscribe((firebaseUser) => {
      console.log('inside the subscription of the authState firebaseUser', firebaseUser);
      this.configureAuthState(firebaseUser);
    });
  }

  doGoogleSignIn(): Promise<void> {
    var googleProvider = new firebase.auth.GoogleAuthProvider();
    googleProvider.addScope('email');
    googleProvider.addScope('profile');
    return this.angularAuth.auth.signInWithPopup(googleProvider).then((auth) => {
      console.log('inside the post sign in method firebaseUser', auth.user);
      this.configureAuthState(auth.user);      
    });
  }

  configureAuthState(firebaseUser: firebase.User): void {
    if (firebaseUser) {
      console.log('firebase user', firebaseUser);
      let theUser = new CurrentUser();
      theUser.displayName = firebaseUser.displayName;
      theUser.email = firebaseUser.email;
      theUser.isSignedIn = true;
      this.user$.next(theUser);
    } else {
      let theUser = new CurrentUser();
      theUser.displayName = null;
      theUser.email = null;
      theUser.isSignedIn = false;
      this.user$.next(theUser);
    }
  }

  logout(): Promise<void> {
    return this.angularAuth.auth.signOut();
  }

  getUserobservable(): Observable<CurrentUser> {
    return this.user$.asObservable();
  }
}

