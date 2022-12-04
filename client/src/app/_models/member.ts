import {Photo} from "./photo";

export interface Member {
    id: string;
    username: string;
    photoUrl: string;
    age: number;
    knownAs: string;
    createdOn: Date;
    lastActive: Date;
    gender: string;
    introduction: string;
    lookingFor: string;
    interests: string;
    city: string;
    country: string;
    photos: Photo[];
}


