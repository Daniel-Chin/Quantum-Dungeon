public class EnumClass {
    private int Id;

    protected EnumClass(int id) {
        Id = id;
    }
    
    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) {
            return false;
        }
        return ((EnumClass) obj).Id == Id;
    }
    public override int GetHashCode()
    {
        return Id;
    }
}
