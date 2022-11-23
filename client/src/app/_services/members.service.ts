import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ResolveEnd } from '@angular/router';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';



@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  user:User;
  userParams: UserParams | undefined;


  constructor(private http: HttpClient,private accountService: AccountService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next:user =>{
        if(user){
          this.userParams= new UserParams(user);
          this.user=user;
        }
      }
    })
  }

  getUserParams(){
    return this.userParams;
  }

  setUserParams(params:UserParams){
    this.userParams = params;
  }

  resetUserParams(){
    if(this.user){
      console.log('in reset filters');
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
    return;
  }

  getMembers(userParams: UserParams) {

    const response = this.memberCache.get(Object.values(userParams).join('-'));

    if (response) return of(response);

    let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);


    return this.getPaginatedResults<Member[]>(this.baseUrl + 'users', params).pipe(
      map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      })
    )
  }

  getMember(username: string) {

    /* const member = this.members.find(x => x.username === username);
    if (member !== undefined) return of(member); */

    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.results), [])
      .find((member: Member) => member.username === username);
      console.log(member);
      if(member) return of(member);
    
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    )

  }

  setMainPhoto(photoId: string) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: string) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  private getPaginatedResults<T>(url: any, params: any) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        paginatedResult.results = response.body as any;
        if (response.headers.get('pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('pagination') as any);
        }
        return paginatedResult;
      })
    );
  }

  private getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());

    return params;
  }
}
