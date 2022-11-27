export interface ChatInfoRequest {
    uerId: number;
    chatId: number;
}

export interface ChatEncodingRequest{
    
    chatId: number;
    userId: number;
    clientsKey: Int8Array
}