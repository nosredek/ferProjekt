import { Rectangle } from '@/models/Rectangle';

export class TaggedImage {
    constructor(
        public id: number,
        public imageBlob: string,
        public taggedFaces: Rectangle[]
    ) { }
}
