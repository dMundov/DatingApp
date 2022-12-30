import { Component, OnInit } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {

  photos: Photo[];

  constructor(private adminService: AdminService) { }

  ngOnInit(): void {
    this.getPhotosForApproval();
  }

  getPhotosForApproval(){
    this.adminService.getPhotosForApproval().subscribe({
       next:photos =>this.photos = photos
    })
  }

  approvePhoto(photoId: string){
    this.adminService.approvePhoto(photoId).subscribe(()=>{
      this.photos.splice(this.photos.findIndex(p=>p.id === photoId),1);
    })
  }

  rejectPhoto(photoId: string){
    this.adminService.rejectPhoto(photoId).subscribe(()=>{
      this.photos.splice(this.photos.findIndex(p=>p.id === photoId),1);
    })
  }

}
