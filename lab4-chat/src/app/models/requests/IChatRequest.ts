export interface ChatInfoRequest {
    userId: number;
    chatId: number;
}

export interface ChatEncodingRequest{
    chatId: number;
    userId: number;
    clientsKey: number[]
}