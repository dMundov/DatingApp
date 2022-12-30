export interface Photo {
    id: string;
    url: string;
    isMain: boolean;
    isApproved:boolean;
    username?:string;
}
