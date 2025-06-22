namespace Game.Websocket.Model
{
    public struct PetEquipItem
    {
        public int petId;
        public string itemId;

        public PetEquipItem(int petId, string itemId)
        {
            this.petId = petId;
            this.itemId = itemId;
        }
    }
}