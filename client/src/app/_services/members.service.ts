import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';



@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environment.apiUrl;
  members: Member[] = [];
  padinatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();

  constructor(private http: HttpClient) { }

  getMembers(page: number | null, itemsPerPage: number | null) {

    let params = new HttpParams();

    if (page !== null && itemsPerPage !== null) {
      params = params.append('pageNumber', page.toString());
      params = params.append('pageSize', itemsPerPage?.toString());
    }
    //if (this.members.length > 0) return of(this.members)
    return this.http.get<Member[]>(this.baseUrl + 'users', {observe:'response', params}).pipe(
      map(response=>{
        this.padinatedResult.results = response.body as any;
        if(response.headers.get('pagination') !== null){
          this.padinatedResult.pagination = JSON.parse(response.headers.get('pagination') as any);
        }
        return this.padinatedResult;
      })
    )
  }

  getMember(username: string | null) {
    const member = this.members.find(x => x.username === username);
    if (member !== undefined) return of(member);
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
}
