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
    this.angularAuth.auth.onAuthStateChanged((firebaseUser) => {
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
    });
  }

  doGoogleSignIn(): Promise<void> {
    var googleProvider = new firebase.auth.GoogleAuthProvider();
    googleProvider.addScope('email');
    googleProvider.addScope('profile');
    return this.angularAuth.auth.signInWithPopup(googleProvider).then(() => { });
  }

  logout(): Promise<void> {
    return this.angularAuth.auth.signOut();
  }

  getUserobservable(): Observable<CurrentUser> {
    return this.user$.asObservable();
  }
}

