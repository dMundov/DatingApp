import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/Message';
import { Pagination } from '../_models/pagination';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages?: Message[];
  pagination?: Pagination;
  container ='Unread';
  pageNUmber = 1;
  pageSize = 5;
  loading = false;

  constructor(private messageService: MessageService) { }

  ngOnInit(): void {
    this.loadMessages();

  }

  loadMessages() {
    this.loading = true;
    this.messageService.getMessages(this.pageNUmber, this.pageSize, this.container).subscribe({
      next: response => {
        this.messages = response.results;
        this.pagination = response.pagination;
        this.loading=false;
      }
    })
  }

  pageChange(event: any) {
    if (this.pageNUmber !== event.page) {
      this.pageNUmber = event.page;
      this.loadMessages();
    }

  }

}
