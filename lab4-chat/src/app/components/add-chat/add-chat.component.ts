import { Component, Inject } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ChatService } from 'src/app/services/chat.service';

@Component({
  selector: 'app-add-chat',
  templateUrl: './add-chat.component.html',
  styleUrls: ['./add-chat.component.scss']
})
export class AddChatComponent {

  chatCreationForm = new FormGroup({
    chatName: new FormControl('', Validators.required),
    userId: new FormControl<number>(0, Validators.required)
  })

  constructor(@Inject(MAT_DIALOG_DATA) public data: { userId: number },
    private chatService: ChatService, private dialogRef: MatDialogRef<AddChatComponent>) {

    this.chatCreationForm.get('userId')?.setValue(data.userId);
  }

  public async addChat() {

    const chatName = this.chatCreationForm.value["chatName"] ?? '';
    const userId = this.chatCreationForm.value["userId"] ?? 0;
    try {
      const response = await this.chatService.createChat(chatName, +userId);
      this.dialogRef.close({ postSubmit: true, response: response })
    } catch (err) {
      this.dialogRef.close({ postSubmit: true, error: err })
    }
  }
}
