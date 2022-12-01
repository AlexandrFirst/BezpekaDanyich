export interface ChatInfoResponse {
    id: number,
    chatPublicKey: number[],
    prime: number,
    name: string,
    primitiveRootModule: number;
}

export interface ChatEncodingResponse {
    encodedEncodingKey: string,
    notEncodedEncodingKey: string
}