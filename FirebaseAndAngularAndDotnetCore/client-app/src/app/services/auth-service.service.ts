import { Injectable } from '@angular/core';
import { AngularFireAuth } from '@angular/fire/auth'
import * as firebase from 'firebase/app';
import { BehaviorSubject, Observable } from 'rxjs';
import { CurrentUser } from '../models/current-user';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})
export class AuthServiceService {

  user$: BehaviorSubject<CurrentUser> = new BehaviorSubject<CurrentUser>(new CurrentUser());

  constructor(private angularAuth: AngularFireAuth, private httpclient: HttpClient) {
    this.angularAuth.authState.subscribe((firebaseUser) => {
      this.configureAuthState(firebaseUser);
    });
  }

  doGoogleSignIn(): Promise<void> {
    var googleProvider = new firebase.auth.GoogleAuthProvider();
    googleProvider.addScope('email');
    googleProvider.addScope('profile');
    return this.angularAuth.auth.signInWithPopup(googleProvider).then((auth) => {});
  }

  configureAuthState(firebaseUser: firebase.User): void {
    if (firebaseUser) {
      firebaseUser.getIdToken().then((theToken) => {
        console.log('we have a token');
        this.httpclient.post('/api/users/verify', { token: theToken }).subscribe({
          next: () => {
            let theUser = new CurrentUser();
            theUser.displayName = firebaseUser.displayName;
            theUser.email = firebaseUser.email;
            theUser.isSignedIn = true;
            localStorage.setItem("jwt", theToken);
            this.user$.next(theUser);
          },
          error: (err) => {
            console.log('inside the error from server', err);
            this.doSignedOutUser()
          }
        });
      }, (failReason) => {
          this.doSignedOutUser();
      });
    } else {
      this.doSignedOutUser();
    }
  }

  private doSignedOutUser() {
    let theUser = new CurrentUser();
    theUser.displayName = null;
    theUser.email = null;
    theUser.isSignedIn = false;
    localStorage.removeItem("jwt");
    this.user$.next(theUser);
  }

  logout(): Promise<void> {
    return this.angularAuth.auth.signOut();
  }

  getUserobservable(): Observable<CurrentUser> {
    return this.user$.asObservable();
  }

  getToken(): string {
    return localStorage.getItem("jwt");
  }

  getUserSecrets(): Observable<string[]> {
    return this.httpclient.get("/api/users/secrets").pipe(map((resp: string[]) => resp));
  }
}

