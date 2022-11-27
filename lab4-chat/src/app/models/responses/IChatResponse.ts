export interface ChatInfoResponse {
    id: number,
    chatPublicKey: Int8Array,
    prime: number,
    name: string,
    primitiveRootModule: number;
}

export interface ChatEncodingResponse {
    encodedEncodingKey: Int8Array
}